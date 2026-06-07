using Auth.Domain.Aggregates;
using Auth.Domain.Filters;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using System.Linq.Expressions;

namespace Auth.Infrastructure.Repositories
{
    public class PermissionGroupRepository : IPermissionGroupRepository
    {
        public Task Create(PermissionGroup entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task CreateRange(IEnumerable<PermissionGroup> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Delete(PermissionGroup entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PermissionGroup?>> Find(Expression<Func<PermissionGroup, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<(IEnumerable<PermissionGroup> Items, int TotalCount)> FindByFilter(PermissionGroupFilter filter, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PermissionGroup?>> GetAll(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PermissionGroup?> GetById(PermissionGroupId id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Update(PermissionGroup entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
