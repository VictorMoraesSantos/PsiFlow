using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Core.Domain.Repositories;

namespace Auth.Domain.Repositories
{
    public interface IPermissionGroupRepository : IRepository<PermissionGroup, PermissionGroupId>
    {
        Task<PermissionGroup?> FindByKey(string groupKey, CancellationToken cancellationToken = default);
    }
}
