using Auth.Domain.Enums;
using Auth.Domain.Errors;
using Auth.Domain.ValueObjects;
using Core.Domain.Aggregates;
using Core.Domain.Exceptions;

namespace Auth.Domain.Entities
{
    public class Permission : BaseEntity<PermissionId>
    {
        public PermissionGroupId PermissionGroupId { get; private set; }
        public PermissionGroup? PermissionGroup { get; private set; }
        public PermissionAction Action { get; private set; }
        public string ClaimValue { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public bool IsActive { get; private set; } = true;

        protected Permission() { }

        public Permission(
            PermissionGroupId permissionGroupId,
            string groupKey,
            PermissionAction action,
            string description)
        {
            if (string.IsNullOrWhiteSpace(groupKey))
                throw new DomainException(PermissionErrors.InvalidKeyGroup);

            PermissionGroupId = permissionGroupId;
            Action = action;
            ClaimValue = BuildClaimValue(groupKey, action);
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

        private static string BuildClaimValue(string groupKey, PermissionAction action)
        {
            return $"{Normalize(groupKey)}.{action.ToString().ToLowerInvariant()}";
        }

        private static string Normalize(string value)
        {
            return value.Trim().ToLowerInvariant().Replace(" ", "_");
        }
    }
}
