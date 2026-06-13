using Core.Domain.Aggregates;

namespace Auth.Domain.Entities
{
    public class RolePermission : BaseEntity<int>
    {
        public string Role { get; private set; } = string.Empty;
        public string Permission { get; private set; } = string.Empty;

        protected RolePermission() { }

        public RolePermission(string role, string permission)
        {
            Role = role;
            Permission = permission;
        }
    }
}
