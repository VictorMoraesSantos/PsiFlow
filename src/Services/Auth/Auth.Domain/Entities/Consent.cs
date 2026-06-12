using Auth.Domain.ValueObjects;
using Core.Domain.Aggregates;

namespace Auth.Domain.Entities
{
    public class Consent : BaseEntity<ConsentId>
    {
        public UserId UserId { get; private set; }
        public TenantId TenantId { get; private set; }
        public string TermsVersion { get; private set; } = string.Empty;
        public string PrivacyVersion { get; private set; } = string.Empty;
        public string DocumentHash { get; private set; } = string.Empty;
        public string? IpAddress { get; private set; }
        public string? UserAgent { get; private set; }
        public DateTime AcceptedAt { get; private set; } = DateTime.UtcNow;

        protected Consent() { }

        public Consent(
            UserId userId,
            TenantId tenantId,
            string termsVersion,
            string privacyVersion,
            string documentHash,
            string? ipAddress,
            string? userAgent,
            DateTime acceptedAt)
        {
            UserId = userId;
            TenantId = tenantId;
            TermsVersion = termsVersion;
            PrivacyVersion = privacyVersion;
            DocumentHash = documentHash;
            IpAddress = ipAddress;
            UserAgent = userAgent;
            AcceptedAt = acceptedAt;
        }
    }
}
