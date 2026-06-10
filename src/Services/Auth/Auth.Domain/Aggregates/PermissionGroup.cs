using Auth.Domain.Enums;
using Auth.Domain.Errors;
using Core.Domain.Aggregates;
using Core.Domain.Exceptions;

namespace Auth.Domain.Aggregates
{
    public class PermissionGroup : BaseEntity<int>
    {
        private readonly List<Permission> _permissions = new();
        public string GroupKey { get; private set; } = string.Empty;
        public string GroupName { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public bool IsActive { get; private set; } = true;
        public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

        protected PermissionGroup() { }

        public PermissionGroup(string groupKey, string groupName, string description)
        {
            SetGroupKey(groupKey);
            SetGroupName(groupName);
            SetDescription(description);
            IsActive = true;
        }

        public void Update(string groupKey, string groupName, string description)
        {
            SetGroupKey(groupKey);
            SetGroupName(groupName);
            SetDescription(description);
            MarkAsUpdated();
        }

        public void Activate() { IsActive = true; MarkAsUpdated(); }
        public void Deactivate()
        {
            IsActive = false;
            foreach (var p in _permissions) p.Deactivate();
            MarkAsUpdated();
        }

        public Permission AddPermission(PermissionAction action, string description)
        {
            if (_permissions.Any(p => p.Action == action))
                throw new DomainException(PermissionErrors.PermissionAlreadyExists);
            var permission = new Permission(Id, GroupKey, action, description);
            _permissions.Add(permission);
            MarkAsUpdated();
            return permission;
        }

        public void RemovePermission(PermissionAction action)
        {
            var p = _permissions.FirstOrDefault(x => x.Action == action);
            if (p is null) return;
            _permissions.Remove(p);
            MarkAsUpdated();
        }

        public void AddDefaultCrudPermissions()
        {
            AddPermission(PermissionAction.View, $"Visualizar {GroupName}");
            AddPermission(PermissionAction.Create, $"Criar {GroupName}");
            AddPermission(PermissionAction.Edit, $"Editar {GroupName}");
            AddPermission(PermissionAction.Delete, $"Excluir {GroupName}");
        }

        private void SetGroupKey(string groupKey)
        {
            if (string.IsNullOrWhiteSpace(groupKey))
                throw new DomainException(PermissionErrors.InvalidGroupKey);
            GroupKey = groupKey.Trim();
        }

        private void SetGroupName(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
                throw new DomainException(PermissionErrors.InvalidGroupName);
            GroupName = groupName.Trim();
        }

        private void SetDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException(PermissionErrors.InvalidDescription);
            Description = description.Trim();
        }
    }
}
