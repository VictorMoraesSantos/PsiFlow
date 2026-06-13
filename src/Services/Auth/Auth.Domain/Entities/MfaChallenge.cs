using Auth.Domain.ValueObjects;
using Core.Domain.Aggregates;

namespace Auth.Domain.Entities
{
    public class MfaChallenge : BaseEntity<MfaChallengeId>
    {
        public UserId UserId { get; private set; }
        public TenantId TenantId { get; private set; }
        public string SecretEncrypted { get; private set; } = string.Empty;
        public string? QrCodeUri { get; private set; }
        public bool IsConfirmed { get; private set; }
        public DateTime? ConfirmedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        protected MfaChallenge() { }

        public MfaChallenge(
            UserId userId,
            TenantId tenantId,
            string secretEncrypted,
            string? qrCodeUri,
            bool isConfirmed,
            DateTime? confirmedAt,
            DateTime expiresAt)
        {
            UserId = userId;
            TenantId = tenantId;
            SecretEncrypted = secretEncrypted;
            QrCodeUri = qrCodeUri;
            IsConfirmed = isConfirmed;
            ConfirmedAt = confirmedAt;
            ExpiresAt = expiresAt;
        }

        public void SetActive(string secretEncrypted, string? qrCodeUri, DateTime expiresAt)
        {
            SecretEncrypted = secretEncrypted;
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
