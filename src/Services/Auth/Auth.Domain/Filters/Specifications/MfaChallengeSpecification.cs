using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Core.Domain.Filters;

namespace Auth.Domain.Filters.Specifications
{
    public class MfaChallengeSpecification : Specification<MfaChallenge, MfaChallengeId>
    {
        public MfaChallengeSpecification(MfaChallengeFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.UserId != null, m => m.UserId == filter.UserId);
            AddIf(filter.TenantId != null, m => m.TenantId == filter.TenantId);
            AddIf(filter.IsConfirmed.HasValue, m => m.IsConfirmed == filter.IsConfirmed!.Value);
        }
    }
}
