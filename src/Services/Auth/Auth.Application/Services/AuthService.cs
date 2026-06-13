using Auth.Application.Authorization;
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
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly EncryptionService _encryption;
        private readonly ILogger<AuthService> _logger;
        private readonly MfaLoginStore _mfaLoginStore;

        public AuthService(
            IUserRepository userRepository,
            IConsentRepository consentRepository,
            IMfaChallengeRepository mfaChallengeRepository,
            IOutboxRepository outboxRepository,
            IRefreshTokenRepository refreshTokenRepository,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ITokenService tokenService,
            EncryptionService encryption,
            ILogger<AuthService> logger,
            MfaLoginStore mfaLoginStore)
        {
            _userRepository = userRepository;
            _consentRepository = consentRepository;
            _mfaChallengeRepository = mfaChallengeRepository;
            _outboxRepository = outboxRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _encryption = encryption;
            _logger = logger;
            _mfaLoginStore = mfaLoginStore;
        }

        public async Task<Result<RegisterResult>> RegisterAsync(RegisterDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto is null)
                return Result.Failure<RegisterResult>(UserErrors.CreateError);

            var email = dto.Email.Trim().ToLowerInvariant();
            var existing = await _userRepository.FindByEmail(email, cancellationToken);
            if (existing != null)
                return Result.Failure<RegisterResult>(UserErrors.RegistrationUnavailable);

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
                return Result.Failure<RegisterResult>(UserErrors.RegistrationUnavailable);
            }

            await _userManager.AddToRoleAsync(user, dto.Role);
            await AssignDefaultPermissionClaimsAsync(user);

            var correlationId = Guid.NewGuid();
            user.AddDomainEvent(new UserRegisteredDomainEvent(user.Id, user.TenantId, user.Email!, user.Role, user.Name.FullName, correlationId));
            user.AddDomainEvent(new ConsentAcceptedDomainEvent(user.Id, user.TenantId, dto.AcceptedTermsVersion, dto.AcceptedPrivacyVersion, DateTime.UtcNow));

            await PersistOutboxAsync(user, correlationId, cancellationToken);

            return Result.Success(new RegisterResult(user.Id, user.TenantId, user.Email!, user.Role));
        }

        public async Task<Result<object>> LoginAsync(LoginDTO dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return Result.Failure<object>(UserErrors.InvalidCredentials);

            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.FindByEmail(email, cancellationToken);
            if (user is null) return Result.Failure<object>(UserErrors.InvalidCredentials);
            if (!user.IsActive) return Result.Failure<object>(UserErrors.AlreadyInactive);
            if (!user.EmailConfirmed) return Result.Failure<object>(UserErrors.EmailNotConfirmed);

            var signIn = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
            if (signIn.IsLockedOut) return Result.Failure<object>(UserErrors.UserLockedOut);
            if (!signIn.Succeeded) return Result.Failure<object>(UserErrors.InvalidCredentials);

            if (user.IsMfaEnabled)
            {
                var challenge = await CreateMfaChallengeAsync(user, cancellationToken);
                var mfaToken = _mfaLoginStore.Create(user.Id, challenge.Id);
                return Result.Success<object>(new MfaRequiredResponse(mfaToken, challenge.Id.ToString()));
            }

            user.UpdateLastLogin();
            await _userRepository.Update(user, cancellationToken);
            return Result.Success<object>(await IssueTokensAsync(user, cancellationToken));
        }

        public async Task<Result<TokenResponse>> CompleteMfaLoginAsync(string mfaToken, string code, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(mfaToken) || string.IsNullOrWhiteSpace(code))
                return Result.Failure<TokenResponse>(UserErrors.MfaCodeInvalid);
            if (!_mfaLoginStore.TryConsume(mfaToken, out var entry))
                return Result.Failure<TokenResponse>(UserErrors.MfaCodeInvalid);

            var user = await _userRepository.GetById(new UserId(entry.UserId), cancellationToken);
            if (user is null) return Result.Failure<TokenResponse>(UserErrors.NotFound(entry.UserId));

            var challenge = await _mfaChallengeRepository.GetById(new MfaChallengeId(entry.ChallengeId), cancellationToken);
            if (challenge is null || challenge.IsExpired(DateTime.UtcNow))
                return Result.Failure<TokenResponse>(UserErrors.MfaChallengeNotFound);
            var secret = _encryption.Decrypt(new EncryptedField(challenge.SecretCiphertext, challenge.SecretNonce, challenge.SecretTag));
            if (!IsValidTotp(secret, code.Trim()))
                return Result.Failure<TokenResponse>(UserErrors.MfaCodeInvalid);

            user.UpdateLastLogin();
            await _userRepository.Update(user, cancellationToken);
            return await IssueTokensAsync(user, cancellationToken);
        }

        public async Task<Result<TokenResponse>> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return Result.Failure<TokenResponse>(UserErrors.RefreshTokenInvalid);

            var hash = _tokenService.HashToken(refreshToken);
            var existing = await _refreshTokenRepository.GetByHashAsync(hash, cancellationToken);
            if (existing is null) return Result.Failure<TokenResponse>(UserErrors.RefreshTokenInvalid);
            if (existing.RevokedAt is not null)
            {
                await RevokeTokenFamilyAsync(existing, cancellationToken);
                return Result.Failure<TokenResponse>(UserErrors.RefreshTokenReused);
            }
            if (existing.ExpiresAt <= DateTime.UtcNow)
            {
                existing.Revoke(DateTime.UtcNow, null, null);
                await _refreshTokenRepository.Update(existing, cancellationToken);
                return Result.Failure<TokenResponse>(UserErrors.RefreshTokenExpired);
            }

            var id = new UserId(existing.UserId);
            var user = await _userRepository.GetById(id, cancellationToken);
            if (user is null) return Result.Failure<TokenResponse>(UserErrors.NotFound(existing.UserId));

            return await IssueTokensAsync(user, existing, cancellationToken);
        }

        public async Task<Result> LogoutAsync(int userId, CancellationToken cancellationToken = default)
        {
            var id = new UserId(userId);
            var user = await _userRepository.GetById(id, cancellationToken);
            if (user is null)
                return Result.Failure(UserErrors.NotFound(userId));

            var active = await _refreshTokenRepository.ListActiveByUserAsync(userId, cancellationToken);
            var now = DateTime.UtcNow;
            foreach (var token in active) token.Revoke(now, null, null);
            await _refreshTokenRepository.UpdateRange(active, cancellationToken);
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

            var documentHash = ComputeHash($"{userId}:{dto.DocumentType}:{dto.TermsVersion}:{dto.PrivacyVersion}");
            var consent = new Consent(new UserId(userId), new TenantId(user.TenantId), dto.DocumentType, $"{dto.TermsVersion}/{dto.PrivacyVersion}", dto.TermsVersion, dto.PrivacyVersion, documentHash, dto.IpAddress, dto.UserAgent, DateTime.UtcNow);
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
            _logger.LogInformation("Reset de senha solicitado para {Email}", email);
            user.AddDomainEvent(new PasswordResetRequestedDomainEvent(user.Id, user.TenantId, user.Email!, token));
            await _userRepository.Update(user, cancellationToken);
            return Result.Success();
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordDTO dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.NewPassword) || !StrongPasswordRegex.IsMatch(dto.NewPassword))
                return Result.Failure(UserErrors.PasswordTooWeak);
            if (dto.NewPassword != dto.ConfirmPassword) return Result.Failure(UserErrors.PasswordsDoNotMatch);

            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.FindByEmail(email, cancellationToken);
            if (user is null) return Result.Failure(UserErrors.PasswordResetInvalid);

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Falha no reset de senha para {Email}: {Errors}", email, errors);
                return Result.Failure(UserErrors.PasswordResetInvalid);
            }

            var activeTokens = await _refreshTokenRepository.ListActiveByUserAsync(user.Id, cancellationToken);
            var now = DateTime.UtcNow;
            foreach (var token in activeTokens) token.Revoke(now, null, null);
            await _refreshTokenRepository.UpdateRange(activeTokens, cancellationToken);
            return Result.Success();
        }

        public async Task<Result<MfaSetupResult>> SetupMfaAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetById(new UserId(userId), cancellationToken);
            if (user is null) return Result.Failure<MfaSetupResult>(UserErrors.NotFound(userId));
            if (user.Role is not ("psychologist" or "saas_admin"))
                return Result.Failure<MfaSetupResult>(UserErrors.MfaNotAllowed);

            var secret = GenerateBase32Secret();
            var expiresAt = DateTime.UtcNow.AddMinutes(10);
            var issuer = Uri.EscapeDataString("PsiFlow");
            var account = Uri.EscapeDataString(user.Email ?? user.Id.ToString());
            var qrCodeUri = $"otpauth://totp/{issuer}:{account}?secret={secret}&issuer={issuer}&digits=6&period=30";

            var encrypted = _encryption.Encrypt(secret);

            var activeChallenge = await _mfaChallengeRepository.GetActiveByUser(userId, cancellationToken);
            if (activeChallenge is not null)
            {
                activeChallenge.SetActive(encrypted.Ciphertext, encrypted.Nonce, encrypted.Tag, qrCodeUri, expiresAt);
                await _mfaChallengeRepository.Update(activeChallenge, cancellationToken);
            }
            else
            {
                await _mfaChallengeRepository.Create(new MfaChallenge(user.Id, user.TenantId, encrypted.Ciphertext, encrypted.Nonce, encrypted.Tag, qrCodeUri, false, null, expiresAt), cancellationToken);
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
            var secret = _encryption.Decrypt(new EncryptedField(challenge.SecretCiphertext, challenge.SecretNonce, challenge.SecretTag));
            if (!IsValidTotp(secret, dto.Code.Trim())) return Result.Failure(UserErrors.MfaCodeInvalid);

            challenge.SetConfirmed();
            await _mfaChallengeRepository.Update(challenge, cancellationToken);

            user.EnableMfa();
            await _userRepository.Update(user, cancellationToken);
            return Result.Success();
        }

        public async Task<Result<string>> RequestEmailVerificationAsync(string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email)) return Result.Failure<string>(ContactErrors.EmailRequired);
            var user = await _userRepository.FindByEmail(email.Trim().ToLowerInvariant(), cancellationToken);
            if (user is null) return Result.Success<string>(string.Empty);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            user.AddDomainEvent(new EmailVerificationRequestedDomainEvent(user.Id, user.TenantId, user.Email!, token));
            return Result.Success(token);
        }

        public async Task<Result> VerifyEmailAsync(string email, string token, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
                return Result.Failure(UserErrors.InvalidCredentials);
            var user = await _userRepository.FindByEmail(email.Trim().ToLowerInvariant(), cancellationToken);
            if (user is null) return Result.Failure(UserErrors.NotFound(0));

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Falha ao verificar e-mail: {Errors}", errors);
                return Result.Failure(UserErrors.InvalidCredentials);
            }
            return Result.Success();
        }

        private async Task<Result<TokenResponse>> IssueTokensAsync(User user, CancellationToken cancellationToken) =>
            await IssueTokensAsync(user, null, cancellationToken);

        private async Task<MfaChallenge> CreateMfaChallengeAsync(User user, CancellationToken cancellationToken)
        {
            var secret = GenerateBase32Secret();
            var encrypted = _encryption.Encrypt(secret);
            var challenge = new MfaChallenge(user.Id, user.TenantId, encrypted.Ciphertext, encrypted.Nonce, encrypted.Tag, null, false, null, DateTime.UtcNow.AddMinutes(10));
            await _mfaChallengeRepository.Create(challenge, cancellationToken);
            return challenge;
        }

        private async Task<Result<TokenResponse>> IssueTokensAsync(User user, RefreshToken? previous, CancellationToken cancellationToken)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var permissions = await _userManager.GetClaimsAsync(user);
            var permissionValues = permissions.Where(c => c.Type == "permission").Select(c => c.Value).ToList();
            var tokenResult = _tokenService.GenerateToken(user.Id, user.Email ?? string.Empty, user.EmailConfirmed, user.TenantId, user.Role, roles, permissionValues);
            if (!tokenResult.IsSuccess) return Result.Failure<TokenResponse>(tokenResult.Error!);

            var refresh = _tokenService.GenerateRefreshToken();
            var expiry = DateTime.UtcNow.AddDays(7);
            var created = new RefreshToken(user.Id, user.TenantId, _tokenService.HashToken(refresh), DateTime.UtcNow, expiry, previous?.CreatedByIp, previous?.UserAgent);
            await _refreshTokenRepository.Create(created, cancellationToken);

            if (previous is not null)
            {
                previous.Revoke(DateTime.UtcNow, previous.CreatedByIp, created.Id);
                await _refreshTokenRepository.Update(previous, cancellationToken);
            }

            return Result.Success(new TokenResponse(tokenResult.Value!, refresh, expiry, new DTOs.Users.UserSummaryDTO(user.Id, user.Name.FullName, user.Email!, user.Role)));
        }

        private async Task RevokeTokenFamilyAsync(RefreshToken reused, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var userTokens = await _refreshTokenRepository.ListActiveByUserAsync(reused.UserId, cancellationToken);
            foreach (var token in userTokens) token.Revoke(now, null, null);
            await _refreshTokenRepository.UpdateRange(userTokens, cancellationToken);
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
            var permissions = user.Role switch
            {
                "saas_admin" => new[] { "*" },
                "psychologist" => PermissionCatalog.PsychologistPermissions(),
                "patient" => PermissionCatalog.PatientPermissions(),
                _ => Array.Empty<string>()
            };

            if (permissions.Length == 0) return;
            if (permissions.Length == 1 && permissions[0] == "*")
            {
                await _userManager.AddClaimAsync(user, new Claim("permission", "*"));
                return;
            }

            foreach (var permission in permissions)
            {
                await _userManager.AddClaimAsync(user, new Claim("permission", permission));
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
