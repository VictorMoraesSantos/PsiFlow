using Core.Domain.Filters;

namespace Auth.Domain.Filters
{
    public class PermissionFilter : DomainQuery
    {
        public int? PermissionGroupId { get; private set; }
        public int? Action { get; private set; }
        public string? GroupKey { get; private set; }
        public string? ClaimType { get; private set; }
        public string? ClaimValue { get; private set; }
        public string? DescriptionContains { get; private set; }
        public bool? IsActive { get; private set; }

        public PermissionFilter(
            int? permissionGroupId = null,
            int? action = null,
            string? groupKey = null,
            string? claimType = null,
            string? claimValue = null,
            string? descriptionContains = null,
            bool? isActive = null)
        {
            PermissionGroupId = permissionGroupId;
            Action = action;
            GroupKey = groupKey;
            ClaimType = claimType;
            ClaimValue = claimValue;
            DescriptionContains = descriptionContains;
            IsActive = isActive;
        }
    }
}
