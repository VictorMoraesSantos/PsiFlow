using Auth.Domain.ValueObjects;
using Core.Domain.Filters;

namespace Auth.Domain.Filters
{
    public class UserFilter : DomainQuery
    {
        public TenantId? TenantId { get; private set; }
        public string? Email { get; private set; }
        public string? NameContains { get; private set; }
        public bool? IsActive { get; private set; }
        public string? Role { get; private set; }

        public UserFilter(
            TenantId? tenantId = null,
            string? email = null,
            string? nameContains = null,
            bool? isActive = null,
            string? role = null)
        {
            TenantId = tenantId;
            Email = email;
            NameContains = nameContains;
            IsActive = isActive;
            Role = role;
        }
    }
}
