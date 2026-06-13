using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using Auth.Domain.Entities;
using Auth.Domain.Errors;
using Auth.Domain.Events;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using BuildingBlocks.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Auth.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConsentRepository _consentRepository;
        private readonly IMfaChallengeRepository _mfaChallengeRepository;
        private readonly IOutboxRepository _outboxRepository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IConsentRepository consentRepository,
            IMfaChallengeRepository mfaChallengeRepository,
            IOutboxRepository outboxRepository,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ITokenService tokenService,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _consentRepository = consentRepository;
            _mfaChallengeRepository = mfaChallengeRepository;
            _outboxRepository = outboxRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<Result<RegisterResult>> RegisterAsync(RegisterDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto is null)
                return Result.Failure<RegisterResult>(UserErrors.CreateError);

            var email = dto.Email.Trim().ToLowerInvariant();
            var existing = await _userRepository.FindByEmail(email, cancellationToken);
            if (existing != null)
                return Result.Failure<RegisterResult>(UserErrors.EmailAlreadyInUse(email));

            var name = string.IsNullOrWhiteSpace(dto.FirstName)
                ? new Name(dto.FullName!)
                : string.IsNullOrWhiteSpace(dto.LastName)
                    ? new Name(dto.FirstName)
                    : new Name(dto.FirstName, dto.LastName);
            var contact = new Contact(dto.Email, dto.Phone);
            var user = new User(name, contact, dto.Role, null, dto.Role == "psychologist" ? dto.Crp!.Trim() : null, dto.AcceptedTermsVersion, dto.AcceptedPrivacyVersion);

            var identity = await _userManager.CreateAsync(user, dto.Password);
            if (!identity.Succeeded)
            {
                var errors = string.Join("; ", identity.Errors.Select(e => e.Description));
                _logger.LogWarning("Falha ao registrar usuario {Email}: {Errors}", email, errors);
                return Result.Failure<RegisterResult>(UserErrors.EmailAlreadyInUse(email));
            }

            await _userManager.AddToRoleAsync(user, dto.Role);
            await AssignDefaultPermissionClaimsAsync(user);

            var correlationId = Guid.NewGuid();
            user.AddDomainEvent(new UserRegisteredDomainEvent(user.Id, user.TenantId, user.Email!, user.Role, user.Name.FullName, correlationId));
            user.AddDomainEvent(new ConsentAcceptedDomainEvent(user.Id, user.TenantId, dto.AcceptedTermsVersion, dto.AcceptedPrivacyVersion, DateTime.UtcNow));

            await PersistOutboxAsync(user, correlationId, cancellationToken);

            return Result.Success(new RegisterResult(user.Id, user.TenantId, user.Email!, user.Role));
        }

        public async Task<Result<TokenResponse>> LoginAsync(LoginDTO dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return Result.Failure<TokenResponse>(UserErrors.InvalidCredentials);

            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.FindByEmail(email, cancellationToken);
            if (user is null) return Result.Failure<TokenResponse>(UserErrors.InvalidCredentials);
            if (!user.IsActive) return Result.Failure<TokenResponse>(UserErrors.AlreadyInactive);

            var signIn = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
            if (signIn.IsLockedOut) return Result.Failure<TokenResponse>(UserErrors.UserLockedOut);
            if (!signIn.Succeeded) return Result.Failure<TokenResponse>(UserErrors.InvalidCredentials);

            user.UpdateLastLogin();
            await _userRepository.Update(user, cancellationToken);

            return await IssueTokensAsync(user, cancellationToken);
        }

        public async Task<Result<TokenResponse>> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return Result.Failure<TokenResponse>(UserErrors.RefreshTokenInvalid);

            var hash = _tokenService.HashToken(refreshToken);
            var user = await _userRepository.FindByRefreshTokenHash(hash, cancellationToken);
            if (user is null) return Result.Failure<TokenResponse>(UserErrors.RefreshTokenReused);
            if (user.RefreshTokenExpiryTime is null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                user.SetRefreshToken(string.Empty, DateTime.MinValue);
                await _userRepository.Update(user, cancellationToken);
                return Result.Failure<TokenResponse>(UserErrors.RefreshTokenExpired);
            }

            return await IssueTokensAsync(user, cancellationToken);
        }

        public async Task<Result> LogoutAsync(int userId, CancellationToken cancellationToken = default)
        {
            var id = new UserId(userId);
            var user = await _userRepository.GetById(id, cancellationToken);
            if (user is null)
                return Result.Failure(UserErrors.NotFound(userId));

            user.SetRefreshToken(string.Empty, DateTime.MinValue);
            await _userRepository.Update(user, cancellationToken);

            return Result.Success();
        }

        public async Task<Result<MeResponse>> MeAsync(int userId, CancellationToken cancellationToken = default)
        {
            var id = new UserId(userId);
            var user = await _userRepository.GetById(id, cancellationToken);
            if (user is null)
                return Result.Failure<MeResponse>(UserErrors.NotFound(userId));

            return Result.Success(new MeResponse(user.Id, user.TenantId, user.Email!, user.Role, user.Name.FullName, user.IsActive));
        }

        public async Task<Result> RecordConsentAsync(int userId, ConsentDTO dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.TermsVersion) || string.IsNullOrWhiteSpace(dto.PrivacyVersion))
                return Result.Failure(UserErrors.TermsNotAccepted);

            var id = new UserId(userId);
            var user = await _userRepository.GetById(id, cancellationToken);
            if (user is null)
                return Result.Failure(UserErrors.NotFound(userId));

            var existing = await _consentRepository.FindByUserAndVersion(userId, dto.TermsVersion, dto.PrivacyVersion, cancellationToken);
            if (existing is not null)
                return Result.Failure(UserErrors.TermsNotAccepted);

            var documentHash = ComputeHash($"{userId}:{dto.TermsVersion}:{dto.PrivacyVersion}");
            var consent = new Consent(new UserId(userId), new TenantId(user.TenantId), dto.TermsVersion, dto.PrivacyVersion, documentHash, null, null, DateTime.UtcNow);
            await _consentRepository.Create(consent, cancellationToken);
            user.RecordConsent(dto.TermsVersion, dto.PrivacyVersion);
            await _userRepository.Update(user, cancellationToken);
            return Result.Success();
        }

        public async Task<Result> ChangePasswordAsync(int userId, ChangePasswordDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto.NewPassword != dto.ConfirmNewPassword)
                return Result.Failure(UserErrors.PasswordsDoNotMatch);

            if (string.IsNullOrWhiteSpace(dto.NewPassword) || !StrongPasswordRegex.IsMatch(dto.NewPassword))
                return Result.Failure(UserErrors.PasswordTooWeak);

            var user = await _userRepository.GetById(new UserId(userId), cancellationToken);
            if (user is null) return Result.Failure(UserErrors.NotFound(userId));

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Falha ao alterar senha do usuario {UserId}: {Errors}", userId, errors);
                return Result.Failure(UserErrors.InvalidCredentials);
            }
            return Result.Success();
        }

        public async Task<Result> ForgotPasswordAsync(ForgotPasswordDTO dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return Result.Failure(ContactErrors.EmailRequired);

            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.FindByEmail(email, cancellationToken);
            if (user is null)
            {
                _logger.LogInformation("ForgotPassword para e-mail inexistente: {Email}", email);
                return Result.Success();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            _logger.LogInformation("Reset token gerado para {Email}: {Token}", email, token);
            return Result.Success();
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordDTO dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.NewPassword) || !StrongPasswordRegex.IsMatch(dto.NewPassword))
                return Result.Failure(UserErrors.PasswordTooWeak);
            if (dto.NewPassword != dto.ConfirmPassword) return Result.Failure(UserErrors.PasswordsDoNotMatch);

            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.FindByEmail(email, cancellationToken);
            if (user is null) return Result.Failure(UserErrors.NotFound(0));

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Falha no reset de senha para {Email}: {Errors}", email, errors);
                return Result.Failure(UserErrors.RefreshTokenInvalid);
            }
            return Result.Success();
        }

        public async Task<Result<MfaSetupResult>> SetupMfaAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetById(new UserId(userId), cancellationToken);
            if (user is null) return Result.Failure<MfaSetupResult>(UserErrors.NotFound(userId));
            if (user.Role is not ("psychologist" or "saas_admin"))
                return Result.Failure<MfaSetupResult>(UserErrors.MfaNotAllowed);

            var secret = GenerateBase32Secret();
            var issuer = Uri.EscapeDataString("PsiFlow");
            var account = Uri.EscapeDataString(user.Email ?? user.Id.ToString());
            var qrCodeUri = $"otpauth://totp/{issuer}:{account}?secret={secret}&issuer={issuer}&digits=6&period=30";

            var activeChallenge = await _mfaChallengeRepository.GetActiveByUser(userId, cancellationToken);
            if (activeChallenge is not null)
            {
                activeChallenge.SetActive(secret, qrCodeUri);
                await _mfaChallengeRepository.Update(activeChallenge, cancellationToken);
            }
            else
            {
                await _mfaChallengeRepository.Create(new MfaChallenge(user.Id, user.TenantId, secret, qrCodeUri, false, null), cancellationToken);
            }

            return Result.Success(new MfaSetupResult(secret, qrCodeUri));
        }

        public async Task<Result> VerifyMfaAsync(int userId, MfaVerifyDTO dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Code))
                return Result.Failure(UserErrors.MfaCodeInvalid);

            var user = await _userRepository.GetById(new UserId(userId), cancellationToken);
            if (user is null) return Result.Failure(UserErrors.NotFound(userId));
            if (user.Role is not ("psychologist" or "saas_admin")) return Result.Failure(UserErrors.MfaNotAllowed);

            var challenge = await _mfaChallengeRepository.GetActiveByUser(userId, cancellationToken);
            if (challenge is null) return Result.Failure(UserErrors.MfaChallengeNotFound);
            if (!IsValidTotp(challenge.SecretEncrypted, dto.Code.Trim())) return Result.Failure(UserErrors.MfaCodeInvalid);

            challenge.SetConfirmed();
            await _mfaChallengeRepository.Update(challenge, cancellationToken);

            user.EnableMfa();
            await _userRepository.Update(user, cancellationToken);
            return Result.Success();
        }

    private async Task<Result<TokenResponse>> IssueTokensAsync(User user, CancellationToken cancellationToken)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var permissions = await _userManager.GetClaimsAsync(user);
        var permissionValues = permissions.Where(c => c.Type == "permission").Select(c => c.Value).ToList();
        var tokenResult = _tokenService.GenerateToken(user.Id, user.Email ?? string.Empty, user.TenantId, user.Role, roles, permissionValues);
        if (!tokenResult.IsSuccess) return Result.Failure<TokenResponse>(tokenResult.Error!);

        var refresh = _tokenService.GenerateRefreshToken();
        var expiry = DateTime.UtcNow.AddDays(7);
        user.SetRefreshToken(_tokenService.HashToken(refresh), expiry);
        await _userRepository.Update(user, cancellationToken);

        return Result.Success(new TokenResponse(tokenResult.Value!, refresh, expiry, new DTOs.Users.UserSummaryDTO(user.Id, user.Name.FullName, user.Email!, user.Role)));
    }

        private async Task PersistOutboxAsync(User user, Guid correlationId, CancellationToken cancellationToken)
        {
            foreach (var evt in user.DomainEvents)
            {
                var outbox = new OutboxMessage(
                    user.Id,
                    nameof(User),
                    evt.GetType().Name,
                    JsonSerializer.Serialize(evt),
                    evt.OccuredOn,
                    null,
                    0,
                    null,
                    correlationId);
                await _outboxRepository.Create(outbox, cancellationToken);
            }
            user.ClearDomainEvents();
            await _userRepository.Update(user, cancellationToken);
        }

        private async Task AssignDefaultPermissionClaimsAsync(User user)
        {
            var groups = user.Role switch
            {
                "saas_admin" => new[] { "*" },
                "psychologist" => new[] { "patients", "sessions", "agenda", "clinical_records", "online_session" },
                "patient" => new[] { "patients", "sessions", "agenda", "online_session" },
                _ => Array.Empty<string>()
            };

            if (groups.Length == 0) return;
            if (groups.Length == 1 && groups[0] == "*")
            {
                await _userManager.AddClaimAsync(user, new Claim("permission", "*"));
                return;
            }

            foreach (var group in groups)
            {
                foreach (var action in new[] { "view", "create", "edit", "delete" })
                {
                    await _userManager.AddClaimAsync(user, new Claim("permission", $"{group}.{action}"));
                }
            }
        }

        private static string ComputeHash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes);
        }

        private static string GenerateBase32Secret()
        {
            var bytes = RandomNumberGenerator.GetBytes(20);
            return ToBase32(bytes);
        }

        private static bool IsValidTotp(string secret, string code)
        {
            if (!Regex.IsMatch(code, "^\\d{6}$")) return false;
            var secretBytes = FromBase32(secret);
            var currentStep = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 30;
            for (var offset = -1; offset <= 1; offset++)
            {
                if (ComputeTotp(secretBytes, currentStep + offset) == code) return true;
            }
            return false;
        }

        private static string ComputeTotp(byte[] secret, long timeStep)
        {
            var counter = BitConverter.GetBytes(timeStep);
            if (BitConverter.IsLittleEndian) Array.Reverse(counter);

            using var hmac = new HMACSHA1(secret);
            var hash = hmac.ComputeHash(counter);
            var offset = hash[^1] & 0x0f;
            var binary = ((hash[offset] & 0x7f) << 24) | ((hash[offset + 1] & 0xff) << 16) | ((hash[offset + 2] & 0xff) << 8) | (hash[offset + 3] & 0xff);
            return (binary % 1_000_000).ToString("D6");
        }

        private static string ToBase32(byte[] bytes)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            var output = new StringBuilder((bytes.Length * 8 + 4) / 5);
            var buffer = 0;
            var bitsLeft = 0;
            foreach (var value in bytes)
            {
                buffer = (buffer << 8) | value;
                bitsLeft += 8;
                while (bitsLeft >= 5)
                {
                    output.Append(alphabet[(buffer >> (bitsLeft - 5)) & 31]);
                    bitsLeft -= 5;
                }
            }
            if (bitsLeft > 0) output.Append(alphabet[(buffer << (5 - bitsLeft)) & 31]);
            return output.ToString();
        }

        private static byte[] FromBase32(string input)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            var bytes = new List<byte>();
            var buffer = 0;
            var bitsLeft = 0;
            foreach (var c in input.TrimEnd('=').ToUpperInvariant())
            {
                var value = alphabet.IndexOf(c);
                if (value < 0) throw new FormatException("Invalid Base32 secret.");
                buffer = (buffer << 5) | value;
                bitsLeft += 5;
                if (bitsLeft >= 8)
                {
                    bytes.Add((byte)((buffer >> (bitsLeft - 8)) & 255));
                    bitsLeft -= 8;
                }
            }
            return bytes.ToArray();
        }

        private static readonly Regex StrongPasswordRegex = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{10,}$", RegexOptions.Compiled);
        private static readonly Regex CrpRegex = new(@"^\d{2}/\d{4,6}$", RegexOptions.Compiled);
    }
}
