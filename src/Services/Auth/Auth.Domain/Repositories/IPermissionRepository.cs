using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Core.Domain.Repositories;

namespace Auth.Domain.Repositories
{
    public interface IPermissionRepository : IRepository<Permission, PermissionId>
    {
        Task<Permission?> FindByClaimValue(string claimValue, CancellationToken cancellationToken = default);
    }
}
