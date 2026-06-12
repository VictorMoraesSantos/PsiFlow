using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Core.Domain.Filters;

namespace Auth.Domain.Filters.Specifications
{
    public class PermissionGroupSpecification : Specification<PermissionGroup, PermissionGroupId>
    {
        public PermissionGroupSpecification(PermissionGroupFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(!string.IsNullOrWhiteSpace(filter.GroupKey), g => g.GroupKey == filter.GroupKey!.Trim());
            AddIf(!string.IsNullOrWhiteSpace(filter.GroupNameContains), g => g.GroupName.Contains(filter.GroupNameContains!.Trim()));
            AddIf(filter.IsActive.HasValue, g => g.IsActive == filter.IsActive!.Value);
        }
    }
}
