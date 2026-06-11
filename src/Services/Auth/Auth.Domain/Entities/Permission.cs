using Auth.Domain.Enums;
using Auth.Domain.Errors;
using Core.Domain.Aggregates;
using Core.Domain.Exceptions;

namespace Auth.Domain.Entities
{
    public class Permission : BaseEntity<int>
    {
        private const string DefaultClaimType = "permission";
        public int PermissionGroupId { get; private set; }
        public PermissionGroup? PermissionGroup { get; private set; }
        public PermissionAction Action { get; private set; }
        public string GroupKey { get; private set; } = string.Empty;
        public string ClaimType { get; private set; } = DefaultClaimType;
        public string ClaimValue { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public bool IsActive { get; private set; } = true;

        protected Permission() { }

        public Permission(int permissionGroupId, string groupKey, PermissionAction action, string description, string claimType = DefaultClaimType)
        {
            if (string.IsNullOrWhiteSpace(groupKey))
                throw new DomainException(PermissionErrors.InvalidKeyGroup);
            if (string.IsNullOrWhiteSpace(claimType))
                throw new DomainException(PermissionErrors.InvalidClaimType);

            PermissionGroupId = permissionGroupId;
            GroupKey = Normalize(groupKey);
            Action = action;
            ClaimType = claimType.Trim().ToLowerInvariant();
            ClaimValue = BuildClaimValue(GroupKey, action);
            Description = description?.Trim() ?? string.Empty;
            IsActive = true;
        }

        public void UpdateDescription(string description)
        {
            Description = description?.Trim() ?? string.Empty;
            MarkAsUpdated();
        }

        public void Activate() { IsActive = true; MarkAsUpdated(); }
        public void Deactivate() { IsActive = false; MarkAsUpdated(); }

        private static string BuildClaimValue(string groupKey, PermissionAction action) =>
            $"{Normalize(groupKey)}.{action.ToString().ToLowerInvariant()}";

        private static string Normalize(string value) =>
            value.Trim().ToLowerInvariant().Replace(" ", "_");
    }
}
