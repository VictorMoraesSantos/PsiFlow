using Auth.Domain.Aggregates;
using Core.Domain.Filters;

namespace Auth.Domain.Filters.Specifications
{
    public class UserSpecification : Specification<User, int>
    {
        public UserSpecification(UserFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.TenantId.HasValue, u => u.TenantId == filter.TenantId!.Value);
            AddIf(!string.IsNullOrWhiteSpace(filter.Email), u => u.Email != null && u.Email == filter.Email!.Trim().ToLowerInvariant());
            AddIf(!string.IsNullOrWhiteSpace(filter.NameContains), u => u.Name.FirstName.Contains(filter.NameContains!.Trim()) || u.Name.LastName.Contains(filter.NameContains!.Trim()));
            AddIf(filter.IsActive.HasValue, u => u.IsActive == filter.IsActive!.Value);
            AddIf(!string.IsNullOrWhiteSpace(filter.Role), u => u.Role == filter.Role);
        }
    }
}
