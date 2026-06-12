using Auth.Domain.ValueObjects;
using Core.Domain.Filters;

namespace Auth.Domain.Filters
{
    public class ConsentFilter : DomainQuery
    {
        public UserId? UserId { get; private set; }
        public TenantId? TenantId { get; private set; }
        public string? TermsVersion { get; private set; }
        public string? PrivacyVersion { get; private set; }
        public string? IpAddress { get; private set; }

        public ConsentFilter(
            UserId? userId = null,
            TenantId? tenantId = null,
            string? termsVersion = null,
            string? privacyVersion = null,
            string? ipAddress = null)
        {
            UserId = userId;
            TenantId = tenantId;
            TermsVersion = termsVersion;
            PrivacyVersion = privacyVersion;
            IpAddress = ipAddress;
        }
    }
}
