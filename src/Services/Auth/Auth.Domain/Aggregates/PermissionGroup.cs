using Auth.Domain.Enums;
using Auth.Domain.Errors;
using Auth.Domain.ValueObjects;
using Core.Domain.Aggregates;
using Core.Domain.Exceptions;

namespace Auth.Domain.Aggregates
{
    public class PermissionGroup : BaseEntity<PermissionGroupId>
    {
        private readonly List<Permission> _permissions = new();
        public string GroupKey { get; private set; }
        public string GroupName { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; } = true;
        public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

        protected PermissionGroup()
        {
        }

        public PermissionGroup(string groupKey, string groupName, string description)
        {
            GroupKey = groupKey;
            GroupName = groupName;
            Description = description;
        }

        public void Update(string groupKey, string groupName, string description)
        {
            SetGroupKey(groupKey);
            SetGroupName(groupName);
            SetDescription(description);
            MarkAsUpdated();
        }

        private void SetGroupKey(string groupKey)
        {
            if (string.IsNullOrWhiteSpace(groupKey))
                throw new DomainException(PermissionErrors.InvalidGroupKey);
            GroupKey = groupKey;
        }

        private void SetGroupName(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
                throw new DomainException(PermissionErrors.InvalidGroupName);
            GroupName = groupName;
        }

        private void SetDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException(PermissionErrors.InvalidDescription);
            Description = description;
        }

        public void Activate()
        {
            IsActive = true;
            MarkAsUpdated();
        }

        public void Deactivate()
        {
            IsActive = false;

            foreach (var permission in _permissions) permission.Deactivate();

            MarkAsUpdated();
        }

        public void AddPermission(PermissionAction action, string description)
        {
            if (_permissions.Any(p => p.Action == action))
                throw new DomainException(PermissionErrors.PermissionAlreadyExists);

            var permission = new Permission(Id.Value, GroupKey, action, description);

            _permissions.Add(permission);

            MarkAsUpdated();
        }

        public void RemovePermission(PermissionAction action)
        {
            var permission = _permissions.FirstOrDefault(p => p.Action == action);
            if (permission == null)
                return;

            _permissions.Remove(permission);
            MarkAsUpdated();
        }

        public void AddDefaultCrudPermissions()
        {
            AddPermission(PermissionAction.View, $"Visualizar {GroupName}");
            AddPermission(PermissionAction.Create, $"Create {GroupName}");
            AddPermission(PermissionAction.Edit, $"Edit {GroupName}");
            AddPermission(PermissionAction.Delete, $"Delete {GroupName}");
        }
    }
}