using Auth.Domain.Errors;
using Auth.Domain.ValueObjects;
using Core.Domain.Aggregates;
using Core.Domain.Exceptions;

namespace Auth.Domain.Entities
{
    public class MfaChallenge : BaseEntity<MfaChallengeId>
    {
        public UserId UserId { get; private set; }
        public TenantId TenantId { get; private set; }
        public string SecretCiphertext { get; private set; } = string.Empty;
        public string SecretNonce { get; private set; } = string.Empty;
        public string SecretTag { get; private set; } = string.Empty;
        public string? QrCodeUri { get; private set; }
        public bool IsConfirmed { get; private set; }
        public DateTime? ConfirmedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        protected MfaChallenge() { }

        protected MfaChallenge(
            UserId userId,
            TenantId tenantId,
            EncryptedField secret,
            string? qrCodeUri,
            DateTime expiresAt)
        {
            UserId = userId;
            TenantId = tenantId;
            SecretCiphertext = secret.Ciphertext;
            SecretNonce = secret.Nonce;
            SecretTag = secret.Tag;
            QrCodeUri = qrCodeUri;
            IsConfirmed = false;
            ConfirmedAt = null;
            ExpiresAt = expiresAt;
        }

        public static MfaChallenge Start(
            UserId userId,
            TenantId tenantId,
            EncryptedField secret,
            string? qrCodeUri,
            TimeSpan lifetime,
            DateTime now)
        {
            if (lifetime <= TimeSpan.Zero)
                throw new DomainException(MfaChallengeErrors.CreateError);
            return new MfaChallenge(userId, tenantId, secret, qrCodeUri, now.Add(lifetime));
        }

        public void SetActive(EncryptedField secret, string? qrCodeUri, DateTime expiresAt)
        {
            if (IsConfirmed)
                throw new DomainException(MfaChallengeErrors.ChallengeAlreadyConfirmed);
            if (expiresAt <= DateTime.UtcNow)
                throw new DomainException(MfaChallengeErrors.ChallengeExpired);

            SecretCiphertext = secret.Ciphertext;
            SecretNonce = secret.Nonce;
            SecretTag = secret.Tag;
            QrCodeUri = qrCodeUri;
            ExpiresAt = expiresAt;
        }

        public bool IsExpired(DateTime now) => ExpiresAt <= now;
        public bool IsUsable(DateTime now) => !IsConfirmed && !IsExpired(now);
        public bool BelongsTo(int userId) => UserId.Value == userId;

        public bool MatchesCode(string decryptedSecretBase32, string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return false;
            return MfaSecret.VerifyCode(decryptedSecretBase32, code.Trim());
        }

        public void Confirm(string decryptedSecretBase32, string code, DateTime now)
        {
            if (IsConfirmed)
                throw new DomainException(MfaChallengeErrors.ChallengeAlreadyConfirmed);
            if (IsExpired(now))
                throw new DomainException(MfaChallengeErrors.ChallengeExpired);
            if (!MatchesCode(decryptedSecretBase32, code))
                throw new DomainException(UserErrors.MfaCodeInvalid);

            IsConfirmed = true;
            ConfirmedAt = now;
            QrCodeUri = null;
        }
    }
}
