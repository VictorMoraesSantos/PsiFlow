using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Core.Domain.Filters;

namespace Auth.Domain.Filters.Specifications
{
    public class ConsentSpecification : Specification<Consent, ConsentId>
    {
        public ConsentSpecification(ConsentFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.UserId != null, c => c.UserId == filter.UserId);
            AddIf(filter.TenantId != null, c => c.TenantId == filter.TenantId);
            AddIf(!string.IsNullOrWhiteSpace(filter.TermsVersion), c => c.TermsVersion == filter.TermsVersion);
            AddIf(!string.IsNullOrWhiteSpace(filter.PrivacyVersion), c => c.PrivacyVersion == filter.PrivacyVersion);
            AddIf(!string.IsNullOrWhiteSpace(filter.IpAddress), c => c.IpAddress == filter.IpAddress);
        }
    }
}
