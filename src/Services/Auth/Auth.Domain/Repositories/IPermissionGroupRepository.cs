using Auth.Domain.Entities;
using Core.Domain.Repositories;

namespace Auth.Domain.Repositories
{
    public interface IPermissionGroupRepository : IRepository<PermissionGroup, int>
    {
        Task<PermissionGroup?> FindByKeyAsync(string groupKey, CancellationToken cancellationToken = default);
    }
}
