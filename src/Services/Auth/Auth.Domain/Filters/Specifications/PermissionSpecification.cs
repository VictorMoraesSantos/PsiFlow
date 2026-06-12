using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Core.Domain.Filters;

namespace Auth.Domain.Filters.Specifications
{
    public class PermissionSpecification : Specification<Permission, PermissionId>
    {
        public PermissionSpecification(PermissionFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.PermissionGroupId != null, p => p.PermissionGroupId == filter.PermissionGroupId);
            AddIf(filter.Action.HasValue, p => p.Action == filter.Action!.Value);
            AddIf(!string.IsNullOrWhiteSpace(filter.GroupKey), p => p.GroupKey == filter.GroupKey!.Trim().ToLowerInvariant().Replace(" ", "_"));
            AddIf(!string.IsNullOrWhiteSpace(filter.ClaimType), p => p.ClaimType == filter.ClaimType!.Trim().ToLowerInvariant());
            AddIf(!string.IsNullOrWhiteSpace(filter.ClaimValue), p => p.ClaimValue == filter.ClaimValue);
            AddIf(filter.IsActive.HasValue, p => p.IsActive == filter.IsActive!.Value);
        }
    }
}
