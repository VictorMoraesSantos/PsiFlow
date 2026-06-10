using Core.Domain.Aggregates;

namespace Auth.Domain.Aggregates
{
    public class Consent : BaseEntity<int>
    {
        public Consent() { }

        public int UserId { get; set; }
        public int TenantId { get; set; }
        public string TermsVersion { get; set; } = string.Empty;
        public string PrivacyVersion { get; set; } = string.Empty;
        public string DocumentHash { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime AcceptedAt { get; set; } = DateTime.UtcNow;
    }
}
