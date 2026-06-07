using Auth.Domain.Aggregates;
using Auth.Domain.ValueObjects;
using Core.Domain.Filters;

namespace Auth.Domain.Filters.Specifications
{
    public class PermissionSpecification : Specification<Permission, PermissionId>
    {
        public PermissionSpecification(PermissionFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.PermissionGroupId.HasValue, p => p.PermissionGroupId.Value == filter.PermissionGroupId!.Value);
            AddIf(filter.Action.HasValue, p => (int)p.Action == filter.Action!);
            AddIf(!string.IsNullOrWhiteSpace(filter.GroupKey), p => p.GroupKey == filter.GroupKey!.Trim().ToLowerInvariant());
            AddIf(!string.IsNullOrWhiteSpace(filter.ClaimType), p => p.ClaimType == filter.ClaimType!.Trim().ToLowerInvariant());
            AddIf(!string.IsNullOrWhiteSpace(filter.ClaimValue), p => p.ClaimValue == filter.ClaimValue!.Trim().ToLowerInvariant());
            AddIf(!string.IsNullOrWhiteSpace(filter.DescriptionContains), p => p.Description.Contains(filter.DescriptionContains!.Trim().ToLowerInvariant()));
            AddIf(filter.IsActive.HasValue, p => p.IsActive == filter.IsActive!.Value);
        }
    }
}
