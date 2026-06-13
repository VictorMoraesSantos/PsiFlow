using Auth.Domain.ValueObjects;
using Core.Domain.Aggregates;

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

        public MfaChallenge(
            UserId userId,
            TenantId tenantId,
            string secretCiphertext,
            string secretNonce,
            string secretTag,
            string? qrCodeUri,
            bool isConfirmed,
            DateTime? confirmedAt,
            DateTime expiresAt)
        {
            UserId = userId;
            TenantId = tenantId;
            SecretCiphertext = secretCiphertext;
            SecretNonce = secretNonce;
            SecretTag = secretTag;
            QrCodeUri = qrCodeUri;
            IsConfirmed = isConfirmed;
            ConfirmedAt = confirmedAt;
            ExpiresAt = expiresAt;
        }

        public void SetActive(string secretCiphertext, string secretNonce, string secretTag, string? qrCodeUri, DateTime expiresAt)
        {
            SecretCiphertext = secretCiphertext;
            SecretNonce = secretNonce;
            SecretTag = secretTag;
            QrCodeUri = qrCodeUri;
            ExpiresAt = expiresAt;
        }

        public bool IsExpired(DateTime now) => ExpiresAt <= now;

        public void SetConfirmed()
        {
            IsConfirmed = true;
            ConfirmedAt = DateTime.UtcNow;
            QrCodeUri = null;
        }
    }
}
