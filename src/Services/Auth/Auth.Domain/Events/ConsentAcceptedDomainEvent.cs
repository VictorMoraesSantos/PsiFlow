using Core.Domain.Events;

namespace Auth.Domain.Events
{
    public sealed class ConsentAcceptedDomainEvent : DomainEvent
    {
        public int UserId { get; }
        public int TenantId { get; }
        public string TermsVersion { get; }
        public string PrivacyVersion { get; }
        public DateTime AcceptedAt { get; }
        public Guid CorrelationId { get; }

        public ConsentAcceptedDomainEvent(int userId, int tenantId, string termsVersion, string privacyVersion, DateTime acceptedAt)
        {
            UserId = userId;
            TenantId = tenantId;
            TermsVersion = termsVersion;
            PrivacyVersion = privacyVersion;
            AcceptedAt = acceptedAt;
            CorrelationId = Guid.NewGuid();
        }
    }
}
