using Auth.Domain.Enums;
using Auth.Domain.ValueObjects;
using Core.Domain.Filters;

namespace Auth.Domain.Filters
{
    public class PermissionFilter : DomainQuery
    {
        public PermissionGroupId? PermissionGroupId { get; private set; }
        public PermissionAction? Action { get; private set; }
        public string? GroupKey { get; private set; }
        public string? ClaimType { get; private set; }
        public string? ClaimValue { get; private set; }
        public bool? IsActive { get; private set; }

        public PermissionFilter(
            PermissionGroupId? permissionGroupId = null,
            PermissionAction? action = null,
            string? groupKey = null,
            string? claimType = null,
            string? claimValue = null,
            bool? isActive = null)
        {
            PermissionGroupId = permissionGroupId;
            Action = action;
            GroupKey = groupKey;
            ClaimType = claimType;
            ClaimValue = claimValue;
            IsActive = isActive;
        }
    }
}
