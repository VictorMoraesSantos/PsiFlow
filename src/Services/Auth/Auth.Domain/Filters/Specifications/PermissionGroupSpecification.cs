using Auth.Domain.Aggregates;
using Auth.Domain.ValueObjects;
using Core.Domain.Filters;

namespace Auth.Domain.Filters.Specifications
{
    public class PermissionGroupSpecification : Specification<PermissionGroup, PermissionGroupId>
    {
        public PermissionGroupSpecification(PermissionGroupFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(!string.IsNullOrWhiteSpace(filter.GroupKeyContains), pg => pg.GroupKey.Contains(filter.GroupKeyContains!.Trim().ToLowerInvariant()));
            AddIf(!string.IsNullOrWhiteSpace(filter.GroupNameContains), pg => pg.GroupName.Contains(filter.GroupNameContains!.Trim().ToLowerInvariant()));
            AddIf(!string.IsNullOrWhiteSpace(filter.DescriptionContains), pg => pg.Description.Contains(filter.DescriptionContains!.Trim().ToLowerInvariant()));
            AddIf(filter.IsActive.HasValue, pg => pg.IsActive == filter.IsActive!.Value);
        }
    }
}
