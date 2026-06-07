using Auth.Domain.Entities;
using Auth.Domain.Filters;
using Auth.Domain.ValueObjects;
using Core.Domain.Repositories;

namespace Auth.Domain.Repositories
{
    public interface IPermissionRepository : IRepository<Permission, PermissionId, PermissionFilter>
    { }
}
