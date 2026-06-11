using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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

        public async Task<Result<RegisterResult>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null) return Result.Failure<RegisterResult>(UserErrors.CreateError);
            if (string.IsNullOrWhiteSpace(request.Role) || (request.Role != "psychologist" && request.Role != "patient" && request.Role != "saas_admin"))
                return Result.Failure<RegisterResult>(UserErrors.RoleInvalid);
            if (string.IsNullOrWhiteSpace(request.AcceptedTermsVersion) || string.IsNullOrWhiteSpace(request.AcceptedPrivacyVersion))
                return Result.Failure<RegisterResult>(UserErrors.TermsNotAccepted);
            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 10 || !StrongPasswordRegex.IsMatch(request.Password))
                return Result.Failure<RegisterResult>(UserErrors.PasswordTooWeak);
            if (request.Password != request.ConfirmPassword)
                return Result.Failure<RegisterResult>(UserErrors.PasswordsDoNotMatch);
            if (request.Role == "psychologist" && string.IsNullOrWhiteSpace(request.Crp))
                return Result.Failure<RegisterResult>(UserErrors.CrpRequired);
            if (request.Role == "psychologist" && !CrpRegex.IsMatch(request.Crp!.Trim()))
                return Result.Failure<RegisterResult>(UserErrors.CrpInvalid);
            if (string.IsNullOrWhiteSpace(request.FullName) || request.FullName.Trim().Length < 2 || request.FullName.Length > 160)
                return Result.Failure<RegisterResult>(NameErrors.TooShort);
            if (request.FullName.Length > 160)
                return Result.Failure<RegisterResult>(NameErrors.TooLong);

            var email = request.Email.Trim().ToLowerInvariant();
            var existing = await _userRepository.FindByEmailAsync(email, cancellationToken);
            if (existing is not null) return Result.Failure<RegisterResult>(UserErrors.EmailAlreadyInUse(email));

            var name = SplitFullName(request.FullName);
            var contact = new Contact(request.Email, request.Phone);
            var user = new User(name, contact, request.Role, null, request.Role == "psychologist" ? request.Crp!.Trim() : null, request.AcceptedTermsVersion, request.AcceptedPrivacyVersion);

            var identity = await _userManager.CreateAsync(user, request.Password);
            if (!identity.Succeeded)
            {
                var errors = string.Join("; ", identity.Errors.Select(e => e.Description));
                _logger.LogWarning("Falha ao registrar usuario {Email}: {Errors}", email, errors);
                return Result.Failure<RegisterResult>(UserErrors.EmailAlreadyInUse(email));
            }

            await _userManager.AddToRoleAsync(user, request.Role);
            if ((request.Role == "psychologist" || request.Role == "saas_admin") && user.TenantId == 0)
            {
                user.AttachTenant(user.Id);
            }

            var correlationId = Guid.NewGuid();
            user.AddDomainEvent(new UserRegisteredDomainEvent(user.Id, user.TenantId, user.Email!, user.Role, user.Name.FullName, correlationId));
            user.AddDomainEvent(new ConsentAcceptedDomainEvent(user.Id, user.TenantId, request.AcceptedTermsVersion, request.AcceptedPrivacyVersion, DateTime.UtcNow));

            await PersistOutboxAsync(user, correlationId, cancellationToken);

            return Result.Success(new RegisterResult(user.Id, user.TenantId, user.Email!, user.Role));
        }

        public async Task<Result<TokenResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return Result.Failure<TokenResponse>(UserErrors.InvalidCredentials);

            var email = request.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.FindByEmailAsync(email, cancellationToken);
            if (user is null) return Result.Failure<TokenResponse>(UserErrors.InvalidCredentials);
            if (!user.IsActive) return Result.Failure<TokenResponse>(UserErrors.AlreadyInactive);

            var signIn = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            if (signIn.IsLockedOut) return Result.Failure<TokenResponse>(UserErrors.UserLockedOut);
            if (!signIn.Succeeded) return Result.Failure<TokenResponse>(UserErrors.InvalidCredentials);

            user.UpdateLastLogin();
            await _userRepository.Update(user, cancellationToken);

            return await IssueTokensAsync(user, cancellationToken);
        }

        public async Task<Result<TokenResponse>> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken)) return Result.Failure<TokenResponse>(UserErrors.RefreshTokenInvalid);
            var hash = _tokenService.HashToken(refreshToken);
            var user = await _userRepository.FindByRefreshTokenHashAsync(hash, cancellationToken);
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
            var user = await _userRepository.GetById(userId, cancellationToken);
            if (user is null) return Result.Failure(UserErrors.NotFound(userId));
            user.SetRefreshToken(string.Empty, DateTime.MinValue);
            await _userRepository.Update(user, cancellationToken);
            return Result.Success();
        }

        public async Task<Result<MeResponse>> MeAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetById(userId, cancellationToken);
            if (user is null) return Result.Failure<MeResponse>(UserErrors.NotFound(userId));
            return Result.Success(new MeResponse(user.Id, user.TenantId, user.Email!, user.Role, user.Name.FullName, user.IsActive));
        }

        public async Task<Result> RecordConsentAsync(int userId, ConsentRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.TermsVersion) || string.IsNullOrWhiteSpace(request.PrivacyVersion))
                return Result.Failure(UserErrors.TermsNotAccepted);
            var user = await _userRepository.GetById(userId, cancellationToken);
            if (user is null) return Result.Failure(UserErrors.NotFound(userId));

            var existing = await _consentRepository.FindByUserAndVersionAsync(userId, request.TermsVersion, request.PrivacyVersion, cancellationToken);
            if (existing is not null) return Result.Failure(UserErrors.TermsNotAccepted);

            var documentHash = ComputeHash($"{userId}:{request.TermsVersion}:{request.PrivacyVersion}");
            var consent = new Consent
            {
                UserId = userId,
                TenantId = user.TenantId,
                TermsVersion = request.TermsVersion,
                PrivacyVersion = request.PrivacyVersion,
                DocumentHash = documentHash,
                AcceptedAt = DateTime.UtcNow
            };
            await _consentRepository.Create(consent, cancellationToken);
            user.RecordConsent(request.TermsVersion, request.PrivacyVersion);
            await _userRepository.Update(user, cancellationToken);
            return Result.Success();
        }

        public async Task<Result> ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
        {
            if (request.NewPassword != request.ConfirmNewPassword) return Result.Failure(UserErrors.PasswordsDoNotMatch);
            if (string.IsNullOrWhiteSpace(request.NewPassword) || !StrongPasswordRegex.IsMatch(request.NewPassword))
                return Result.Failure(UserErrors.PasswordTooWeak);

            var user = await _userRepository.GetById(userId, cancellationToken);
            if (user is null) return Result.Failure(UserErrors.NotFound(userId));

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Falha ao alterar senha do usuario {UserId}: {Errors}", userId, errors);
                return Result.Failure(UserErrors.InvalidCredentials);
            }
            return Result.Success();
        }

        public async Task<Result> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Email)) return Result.Failure(ContactErrors.EmailRequired);
            var email = request.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.FindByEmailAsync(email, cancellationToken);
            if (user is null)
            {
                _logger.LogInformation("ForgotPassword para e-mail inexistente: {Email}", email);
                return Result.Success();
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            _logger.LogInformation("Reset token gerado para {Email}: {Token}", email, token);
            return Result.Success();
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.NewPassword) || !StrongPasswordRegex.IsMatch(request.NewPassword))
                return Result.Failure(UserErrors.PasswordTooWeak);
            if (request.NewPassword != request.ConfirmPassword) return Result.Failure(UserErrors.PasswordsDoNotMatch);

            var email = request.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.FindByEmailAsync(email, cancellationToken);
            if (user is null) return Result.Failure(UserErrors.NotFound(0));

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
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
            var user = await _userRepository.GetById(userId, cancellationToken);
            if (user is null) return Result.Failure<MfaSetupResult>(UserErrors.NotFound(userId));
            if (user.Role is not ("psychologist" or "saas_admin"))
                return Result.Failure<MfaSetupResult>(UserErrors.MfaNotAllowed);

            var secret = GenerateBase32Secret();
            var issuer = Uri.EscapeDataString("PsiFlow");
            var account = Uri.EscapeDataString(user.Email ?? user.Id.ToString());
            var qrCodeUri = $"otpauth://totp/{issuer}:{account}?secret={secret}&issuer={issuer}&digits=6&period=30";

            var activeChallenge = await _mfaChallengeRepository.GetActiveByUserAsync(userId, cancellationToken);
            if (activeChallenge is not null)
            {
                activeChallenge.SecretEncrypted = secret;
                activeChallenge.QrCodeUri = qrCodeUri;
                await _mfaChallengeRepository.Update(activeChallenge, cancellationToken);
            }
            else
            {
                await _mfaChallengeRepository.Create(new MfaChallenge
                {
                    UserId = user.Id,
                    TenantId = user.TenantId,
                    SecretEncrypted = secret,
                    QrCodeUri = qrCodeUri,
                    IsConfirmed = false
                }, cancellationToken);
            }

            return Result.Success(new MfaSetupResult(secret, qrCodeUri));
        }

        public async Task<Result> VerifyMfaAsync(int userId, MfaVerifyRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Code)) return Result.Failure(UserErrors.MfaCodeInvalid);

            var user = await _userRepository.GetById(userId, cancellationToken);
            if (user is null) return Result.Failure(UserErrors.NotFound(userId));
            if (user.Role is not ("psychologist" or "saas_admin")) return Result.Failure(UserErrors.MfaNotAllowed);

            var challenge = await _mfaChallengeRepository.GetActiveByUserAsync(userId, cancellationToken);
            if (challenge is null) return Result.Failure(UserErrors.MfaChallengeNotFound);
            if (!IsValidTotp(challenge.SecretEncrypted, request.Code.Trim())) return Result.Failure(UserErrors.MfaCodeInvalid);

            challenge.IsConfirmed = true;
            challenge.ConfirmedAt = DateTime.UtcNow;
            await _mfaChallengeRepository.Update(challenge, cancellationToken);

            user.EnableMfa();
            await _userRepository.Update(user, cancellationToken);
            return Result.Success();
        }

        private async Task<Result<TokenResponse>> IssueTokensAsync(User user, CancellationToken cancellationToken)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var tokenResult = _tokenService.GenerateToken(user.Id, user.Email ?? string.Empty, user.TenantId, user.Role, roles);
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
                var outbox = new OutboxMessage
                {
                    AggregateId = user.Id,
                    AggregateType = nameof(User),
                    EventType = evt.GetType().Name,
                    Payload = System.Text.Json.JsonSerializer.Serialize(evt),
                    OccurredAt = evt.OccuredOn,
                    CorrelationId = correlationId
                };
                await _outboxRepository.Create(outbox, cancellationToken);
            }
            user.ClearDomainEvents();
            await _userRepository.Update(user, cancellationToken);
        }

        private static Name SplitFullName(string fullName)
        {
            var parts = fullName.Trim().Split(' ', 2);
            return new Name(parts[0], parts.Length > 1 ? parts[1] : string.Empty);
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
