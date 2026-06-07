using Auth.Domain.Enums;
using Auth.Domain.Errors;
using Auth.Domain.ValueObjects;
using Core.Domain.Aggregates;
using Core.Domain.Exceptions;

namespace Auth.Domain.Aggregates
{
    public class Permission : BaseEntity<PermissionId>
    {
        private const string DefaultClaimType = "permission";
        public PermissionGroupId PermissionGroupId { get; private set; }
        public PermissionGroup PermissionGroup { get; private set; }
        public PermissionAction Action { get; private set; }
        public string GroupKey { get; }
        public string ClaimType { get; private set; } = DefaultClaimType;
        public string ClaimValue { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; } = true;

        protected Permission()
        { }

        public Permission(
            int permissionGroupId,
            string groupKey,
            PermissionAction action,
            string description,
            string claimType = DefaultClaimType)
        {
            if (string.IsNullOrWhiteSpace(groupKey))
                throw new DomainException(PermissionErrors.InvalidKeyGroup);

            if (string.IsNullOrWhiteSpace(claimType))
                throw new DomainException(PermissionErrors.InvalidClaimType);

            PermissionGroupId = new PermissionGroupId(permissionGroupId);
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

        public void Activate()
        {
            IsActive = true;
            MarkAsUpdated();
        }

        public void Deactivate()
        {
            IsActive = false;
            MarkAsUpdated();
        }

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