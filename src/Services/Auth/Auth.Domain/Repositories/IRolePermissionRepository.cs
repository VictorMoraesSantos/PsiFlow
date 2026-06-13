using Auth.Domain.Entities;
using Core.Domain.Repositories;

namespace Auth.Domain.Repositories
{
    public interface IRolePermissionRepository : IRepository<RolePermission, int>
    {
        Task<IReadOnlyList<string>> GetPermissionsForRoleAsync(string role, CancellationToken cancellationToken = default);
    }
}
