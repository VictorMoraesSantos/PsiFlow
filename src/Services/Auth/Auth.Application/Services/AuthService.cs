using Auth.Application.Authorization;
using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using Auth.Application.Settings;
using Auth.Domain.Entities;
using Auth.Domain.Errors;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using BuildingBlocks.Results;
using Core.Domain.Events;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Json;
using DomainEncryptedField = Auth.Domain.ValueObjects.EncryptedField;

namespace Auth.Application.Services
{
    public class AuthService : IAuthService
    {
        private const int RefreshTokenLifetimeDays = 7;
        private static readonly TimeSpan MfaChallengeLifetime = TimeSpan.FromMinutes(10);

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
        private readonly AuthOptions _authOptions;

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
            MfaLoginStore mfaLoginStore,
            IOptions<AuthOptions> authOptions)
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
            _authOptions = authOptions.Value;
        }

        public async Task<Result<RegisterResult>> RegisterAsync(RegisterDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto is null)
                return Result.Failure<RegisterResult>(UserErrors.CreateError);

            var email = dto.Email.Trim().ToLowerInvariant();
            if (await _userRepository.FindByEmail(email, cancellationToken) is not null)
                return Result.Failure<RegisterResult>(UserErrors.RegistrationUnavailable);

            var nameResult = TryBuildName(dto);
            if (!nameResult.IsSuccess) return Result.Failure<RegisterResult>(nameResult.Error!);

            Contact contact;
            try
            {
                contact = new Contact(dto.Email, dto.Phone);
            }
            catch (Exception ex)
            {
                return Result.Failure<RegisterResult>(ContactErrors.InvalidFormat);
            }

            DocumentVersion termsVersion;
            DocumentVersion privacyVersion;
            try
            {
                termsVersion = DocumentVersion.Create(dto.AcceptedTermsVersion, nameof(dto.AcceptedTermsVersion));
                privacyVersion = DocumentVersion.Create(dto.AcceptedPrivacyVersion, nameof(dto.AcceptedPrivacyVersion));
            }
            catch (Exception ex)
            {
                return Result.Failure<RegisterResult>(UserErrors.TermsNotAccepted);
            }

            var user = User.Register(
                nameResult.Value!,
                contact,
                dto.Role,
                tenantId: null,
                crp: dto.Role == UserRole.Psychologist ? dto.Crp : null,
                termsVersion: termsVersion,
                privacyVersion: privacyVersion);

            var identity = await _userManager.CreateAsync(user, dto.Password);
            if (!identity.Succeeded)
            {
                _logger.LogWarning("Falha ao registrar usuario {Email}: {Errors}", email, string.Join("; ", identity.Errors.Select(e => e.Description)));
                return Result.Failure<RegisterResult>(UserErrors.RegistrationUnavailable);
            }

            await _userManager.AddToRoleAsync(user, user.Role);
            await AssignDefaultPermissionClaimsAsync(user);

            if (_authOptions.AutoConfirmEmails)
            {
                user.ConfirmEmail();
                var emailUpdate = await _userManager.UpdateAsync(user);
                if (!emailUpdate.Succeeded)
                    _logger.LogWarning("Falha ao auto-confirmar e-mail do usuario {UserId}: {Errors}", user.Id, string.Join("; ", emailUpdate.Errors.Select(e => e.Description)));
            }

            var correlationId = Guid.NewGuid();
            user.RecordConsent(termsVersion, privacyVersion);
            user.RegisterUser(correlationId);

            await PersistOutboxAsync(user, correlationId, cancellationToken);

            return Result.Success(new RegisterResult(user.Id, user.TenantId, user.Email!, user.Role));
        }

        public async Task<Result<object>> LoginAsync(LoginDTO dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return Result.Failure<object>(UserErrors.InvalidCredentials);

            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.FindByEmail(email, cancellationToken);
            if (user is null)
            {
                _logger.LogInformation("Login falhou: usuario nao encontrado para {Email}", email);
                return Result.Failure<object>(UserErrors.InvalidCredentials);
            }
            if (!user.IsActive)
            {
                _logger.LogInformation("Login falhou: usuario inativo {UserId}", user.Id);
                return Result.Failure<object>(UserErrors.AlreadyInactive);
            }
            if (!user.EmailConfirmed)
            {
                _logger.LogInformation("Login falhou: e-mail nao confirmado {UserId}", user.Id);
                return Result.Failure<object>(UserErrors.EmailNotConfirmed);
            }

            var signIn = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
            if (signIn.IsLockedOut)
            {
                _logger.LogInformation("Login falhou: conta bloqueada {UserId} ate {LockoutEnd}", user.Id, user.LockoutEnd);
                return Result.Failure<object>(UserErrors.UserLockedOut);
            }
            if (!signIn.Succeeded)
            {
                _logger.LogInformation("Login falhou: senha invalida para {UserId}", user.Id);
                return Result.Failure<object>(UserErrors.InvalidCredentials);
            }

            if (user.IsMfaEnabled)
            {
                var challenge = await StartMfaChallengeAsync(user, cancellationToken);
                var mfaToken = _mfaLoginStore.Create(user.Id, challenge.Id);
                return Result.Success<object>(new MfaRequiredResponse(mfaToken, challenge.Id.ToString()));
            }

            user.BeginLogin();
            await _userRepository.Update(user, cancellationToken);
            return Result.Success<object>(await IssueTokensAsync(user, previous: null, cancellationToken));
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
            if (challenge is null) return Result.Failure<TokenResponse>(UserErrors.MfaChallengeNotFound);
            if (!challenge.BelongsTo(user.Id)) return Result.Failure<TokenResponse>(UserErrors.MfaCodeInvalid);
            if (!challenge.IsUsable(DateTime.UtcNow))
                return Result.Failure<TokenResponse>(UserErrors.MfaChallengeNotFound);

            var secret = DecryptSecret(challenge);
            challenge.Confirm(secret, code.Trim(), DateTime.UtcNow);
            await _mfaChallengeRepository.Update(challenge, cancellationToken);

            user.BeginLogin();
            await _userRepository.Update(user, cancellationToken);
            return await IssueTokensAsync(user, previous: null, cancellationToken);
        }

        public async Task<Result<TokenResponse>> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return Result.Failure<TokenResponse>(UserErrors.RefreshTokenInvalid);

            var hash = _tokenService.HashToken(refreshToken);
            var existing = await _refreshTokenRepository.GetByHashAsync(hash, cancellationToken);
            if (existing is null) return Result.Failure<TokenResponse>(UserErrors.RefreshTokenInvalid);
            if (!existing.BelongsTo(existing.UserId))
                return Result.Failure<TokenResponse>(UserErrors.RefreshTokenInvalid);

            if (existing.IsRevoked())
            {
                await RevokeTokenFamilyAsync(existing, cancellationToken);
                return Result.Failure<TokenResponse>(UserErrors.RefreshTokenReused);
            }

            if (existing.IsExpired(DateTime.UtcNow))
            {
                existing.Revoke(DateTime.UtcNow, revokedByIp: null, replacedByTokenId: null);
                await _refreshTokenRepository.Update(existing, cancellationToken);
                return Result.Failure<TokenResponse>(UserErrors.RefreshTokenExpired);
            }

            var user = await _userRepository.GetById(new UserId(existing.UserId), cancellationToken);
            if (user is null) return Result.Failure<TokenResponse>(UserErrors.NotFound(existing.UserId));

            return await IssueTokensAsync(user, existing, cancellationToken);
        }

        public async Task<Result> LogoutAsync(int userId, CancellationToken cancellationToken = default)
        {
            var id = new UserId(userId);
            var user = await _userRepository.GetById(id, cancellationToken);
            if (user is null) return Result.Failure(UserErrors.NotFound(userId));

            var active = await _refreshTokenRepository.ListActiveByUserAsync(userId, cancellationToken);
            var now = DateTime.UtcNow;
            foreach (var token in active) token.Revoke(now, revokedByIp: null, replacedByTokenId: null);
            await _refreshTokenRepository.UpdateRange(active, cancellationToken);
            return Result.Success();
        }

        public async Task<Result<MeResponse>> MeAsync(int userId, CancellationToken cancellationToken = default)
        {
            var id = new UserId(userId);
            var user = await _userRepository.GetById(id, cancellationToken);
            if (user is null) return Result.Failure<MeResponse>(UserErrors.NotFound(userId));

            return Result.Success(new MeResponse(user.Id, user.TenantId, user.Email!, user.Role, user.Name.FullName, user.IsActive));
        }

        public async Task<Result> RecordConsentAsync(int userId, ConsentDTO dto, CancellationToken cancellationToken = default)
        {
            DocumentVersion termsVersion;
            DocumentVersion privacyVersion;
            try
            {
                termsVersion = DocumentVersion.Create(dto.TermsVersion, nameof(dto.TermsVersion));
                privacyVersion = DocumentVersion.Create(dto.PrivacyVersion, nameof(dto.PrivacyVersion));
            }
            catch (Exception)
            {
                return Result.Failure(UserErrors.TermsNotAccepted);
            }

            var id = new UserId(userId);
            var user = await _userRepository.GetById(id, cancellationToken);
            if (user is null) return Result.Failure(UserErrors.NotFound(userId));

            var existing = await _consentRepository.FindByUserAndVersion(userId, termsVersion.Value, privacyVersion.Value, cancellationToken);
            if (existing is not null) return Result.Failure(UserErrors.TermsNotAccepted);

            var consent = Consent.Accept(
                new UserId(userId),
                new TenantId(user.TenantId),
                termsVersion,
                privacyVersion,
                dto.DocumentType,
                dto.IpAddress,
                dto.UserAgent,
                DateTime.UtcNow);
            await _consentRepository.Create(consent, cancellationToken);

            user.RecordConsent(termsVersion, privacyVersion);
            await _userRepository.Update(user, cancellationToken);
            return Result.Success();
        }

        public async Task<Result> ChangePasswordAsync(int userId, ChangePasswordDTO dto, CancellationToken cancellationToken = default)
        {
            PasswordPolicy newPassword;
            PasswordPolicy confirmation;
            try
            {
                newPassword = PasswordPolicy.Create(dto.NewPassword);
                confirmation = PasswordPolicy.Create(dto.ConfirmNewPassword);
                PasswordPolicy.EnsureMatch(newPassword, confirmation);
            }
            catch (Exception ex)
            {
                var error = ex.Message.Contains("nao conferem", StringComparison.OrdinalIgnoreCase)
                    ? UserErrors.PasswordsDoNotMatch
                    : UserErrors.PasswordTooWeak;
                return Result.Failure(error);
            }

            var user = await _userRepository.GetById(new UserId(userId), cancellationToken);
            if (user is null) return Result.Failure(UserErrors.NotFound(userId));

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, newPassword.Value);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Falha ao alterar senha do usuario {UserId}: {Errors}", userId, string.Join("; ", result.Errors.Select(e => e.Description)));
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
            user.RequestPasswordReset(token);
            await _userRepository.Update(user, cancellationToken);
            return Result.Success();
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordDTO dto, CancellationToken cancellationToken = default)
        {
            PasswordPolicy newPassword;
            PasswordPolicy confirmation;
            try
            {
                newPassword = PasswordPolicy.Create(dto.NewPassword);
                confirmation = PasswordPolicy.Create(dto.ConfirmPassword);
                PasswordPolicy.EnsureMatch(newPassword, confirmation);
            }
            catch (Exception ex)
            {
                var error = ex.Message.Contains("nao conferem", StringComparison.OrdinalIgnoreCase)
                    ? UserErrors.PasswordsDoNotMatch
                    : UserErrors.PasswordTooWeak;
                return Result.Failure(error);
            }

            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.FindByEmail(email, cancellationToken);
            if (user is null) return Result.Failure(UserErrors.PasswordResetInvalid);

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, newPassword.Value);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Falha no reset de senha para {Email}: {Errors}", email, string.Join("; ", result.Errors.Select(e => e.Description)));
                return Result.Failure(UserErrors.PasswordResetInvalid);
            }

            var activeTokens = await _refreshTokenRepository.ListActiveByUserAsync(user.Id, cancellationToken);
            var now = DateTime.UtcNow;
            foreach (var token in activeTokens) token.Revoke(now, revokedByIp: null, replacedByTokenId: null);
            await _refreshTokenRepository.UpdateRange(activeTokens, cancellationToken);
            return Result.Success();
        }

        public async Task<Result<MfaSetupResult>> SetupMfaAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetById(new UserId(userId), cancellationToken);
            if (user is null) return Result.Failure<MfaSetupResult>(UserErrors.NotFound(userId));
            if (!user.IsMfaEligible()) return Result.Failure<MfaSetupResult>(UserErrors.MfaNotAllowed);

            var secret = MfaSecret.Generate();
            var qrCodeUri = secret.BuildQrCodeUri(user.Email ?? user.Id.ToString());
            var encrypted = _encryption.Encrypt(secret.Base32);
            var encryptedField = new DomainEncryptedField(encrypted.Ciphertext, encrypted.Nonce, encrypted.Tag);

            var expiresAt = DateTime.UtcNow.Add(MfaChallengeLifetime);
            var activeChallenge = await _mfaChallengeRepository.GetActiveByUser(userId, cancellationToken);
            if (activeChallenge is not null)
            {
                activeChallenge.SetActive(encryptedField, qrCodeUri, expiresAt);
                await _mfaChallengeRepository.Update(activeChallenge, cancellationToken);
            }
            else
            {
                var challenge = MfaChallenge.Start(user.Id, user.TenantId, encryptedField, qrCodeUri, MfaChallengeLifetime, DateTime.UtcNow);
                await _mfaChallengeRepository.Create(challenge, cancellationToken);
            }

            return Result.Success(new MfaSetupResult(secret.Base32, qrCodeUri));
        }

        public async Task<Result> VerifyMfaAsync(int userId, MfaVerifyDTO dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Code))
                return Result.Failure(UserErrors.MfaCodeInvalid);

            var user = await _userRepository.GetById(new UserId(userId), cancellationToken);
            if (user is null) return Result.Failure(UserErrors.NotFound(userId));
            if (!user.IsMfaEligible()) return Result.Failure(UserErrors.MfaNotAllowed);

            var challenge = await _mfaChallengeRepository.GetActiveByUser(userId, cancellationToken);
            if (challenge is null) return Result.Failure(UserErrors.MfaChallengeNotFound);

            var secret = DecryptSecret(challenge);
            challenge.Confirm(secret, dto.Code.Trim(), DateTime.UtcNow);
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
            user.RequestEmailVerification(token);
            await _userRepository.Update(user, cancellationToken);
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
                _logger.LogWarning("Falha ao verificar e-mail: {Errors}", string.Join("; ", result.Errors.Select(e => e.Description)));
                return Result.Failure(UserErrors.InvalidCredentials);
            }
            user.ConfirmEmail();
            await _userRepository.Update(user, cancellationToken);
            return Result.Success();
        }

        private async Task<MfaChallenge> StartMfaChallengeAsync(User user, CancellationToken cancellationToken)
        {
            var secret = MfaSecret.Generate();
            var encrypted = _encryption.Encrypt(secret.Base32);
            var encryptedField = new DomainEncryptedField(encrypted.Ciphertext, encrypted.Nonce, encrypted.Tag);
            var challenge = MfaChallenge.Start(user.Id, user.TenantId, encryptedField, qrCodeUri: null, MfaChallengeLifetime, DateTime.UtcNow);
            await _mfaChallengeRepository.Create(challenge, cancellationToken);
            return challenge;
        }

        private string DecryptSecret(MfaChallenge challenge)
        {
            var encrypted = new DomainEncryptedField(challenge.SecretCiphertext, challenge.SecretNonce, challenge.SecretTag);
            return _encryption.Decrypt(encrypted);
        }

        private async Task<Result<TokenResponse>> IssueTokensAsync(User user, RefreshToken? previous, CancellationToken cancellationToken)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var permissions = await _userManager.GetClaimsAsync(user);
            var permissionValues = permissions.Where(c => c.Type == "permission").Select(c => c.Value).ToList();
            var tokenResult = _tokenService.GenerateToken(user.Id, user.Email ?? string.Empty, user.EmailConfirmed, user.TenantId, user.Role, roles, permissionValues);
            if (!tokenResult.IsSuccess) return Result.Failure<TokenResponse>(tokenResult.Error!);

            var now = DateTime.UtcNow;
            var rawRefresh = _tokenService.GenerateRefreshToken();
            var refreshHash = _tokenService.HashToken(rawRefresh);
            var lifetime = TimeSpan.FromDays(RefreshTokenLifetimeDays);

            RefreshToken issued;
            if (previous is null)
            {
                issued = RefreshToken.Issue(user.Id, user.TenantId, refreshHash, now, lifetime, createdByIp: null, userAgent: null);
                await _refreshTokenRepository.Create(issued, cancellationToken);
            }
            else
            {
                issued = previous.Rotate(refreshHash, now, lifetime);
                await _refreshTokenRepository.Update(previous, cancellationToken);
                await _refreshTokenRepository.Create(issued, cancellationToken);
            }

            return Result.Success(new TokenResponse(
                tokenResult.Value!,
                rawRefresh,
                issued.ExpiresAt,
                new DTOs.Users.UserSummaryDTO(user.Id, user.Name.FullName, user.Email!, user.Role)));
        }

        private async Task RevokeTokenFamilyAsync(RefreshToken reused, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var userTokens = await _refreshTokenRepository.ListActiveByUserAsync(reused.UserId, cancellationToken);
            foreach (var token in userTokens) token.Revoke(now, revokedByIp: null, replacedByTokenId: null);
            await _refreshTokenRepository.UpdateRange(userTokens, cancellationToken);
        }

        private async Task PersistOutboxAsync(User user, Guid correlationId, CancellationToken cancellationToken)
        {
            foreach (var evt in user.DomainEvents)
            {
                var outbox = OutboxMessage.FromDomainEvent(user.Id, nameof(User), evt, correlationId);
                await _outboxRepository.Create(outbox, cancellationToken);
            }
            user.ClearDomainEvents();
            await _userRepository.Update(user, cancellationToken);
        }

        private async Task AssignDefaultPermissionClaimsAsync(User user)
        {
            var permissions = user.Role switch
            {
                UserRole.SaasAdmin => new[] { "*" },
                UserRole.Psychologist => PermissionCatalog.PsychologistPermissions(),
                UserRole.Patient => PermissionCatalog.PatientPermissions(),
                _ => Array.Empty<string>()
            };

            if (permissions.Length == 0) return;
            if (permissions.Length == 1 && permissions[0] == "*")
            {
                await _userManager.AddClaimAsync(user, new Claim("permission", "*"));
                return;
            }

            foreach (var permission in permissions)
                await _userManager.AddClaimAsync(user, new Claim("permission", permission));
        }

        private static Result<Name> TryBuildName(RegisterDTO dto)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(dto.FirstName) && !string.IsNullOrWhiteSpace(dto.LastName))
                    return Result.Success(new Name(dto.FirstName, dto.LastName));
                if (!string.IsNullOrWhiteSpace(dto.FirstName))
                    return Result.Success(new Name(dto.FirstName));
                if (!string.IsNullOrWhiteSpace(dto.FullName))
                    return Result.Success(new Name(dto.FullName));
                return Result.Failure<Name>(NameErrors.NullName);
            }
            catch (Exception)
            {
                return Result.Failure<Name>(NameErrors.NullName);
            }
        }
    }
}
