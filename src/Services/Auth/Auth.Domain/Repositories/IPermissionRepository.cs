using Auth.Domain.Aggregates;
using Core.Domain.Repositories;

namespace Auth.Domain.Repositories
{
    public interface IPermissionRepository : IRepository<Permission, int>
    {
        Task<Permission?> FindByClaimAsync(string claimType, string claimValue, CancellationToken cancellationToken = default);
    }
}
