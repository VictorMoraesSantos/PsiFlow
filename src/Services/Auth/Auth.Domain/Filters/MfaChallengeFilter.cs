using Auth.Domain.ValueObjects;
using Core.Domain.Filters;

namespace Auth.Domain.Filters
{
    public class MfaChallengeFilter : DomainQuery
    {
        public UserId? UserId { get; private set; }
        public TenantId? TenantId { get; private set; }
        public bool? IsConfirmed { get; private set; }

        public MfaChallengeFilter(
            UserId? userId = null,
            TenantId? tenantId = null,
            bool? isConfirmed = null)
        {
            UserId = userId;
            TenantId = tenantId;
            IsConfirmed = isConfirmed;
        }
    }
}
