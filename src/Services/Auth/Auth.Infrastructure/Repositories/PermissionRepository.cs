using Auth.Domain.Aggregates;
using Auth.Domain.Filters;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using System.Linq.Expressions;

namespace Auth.Infrastructure.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        public Task Create(Permission entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task CreateRange(IEnumerable<Permission> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Permission entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Permission?>> Find(Expression<Func<Permission, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<(IEnumerable<Permission> Items, int TotalCount)> FindByFilter(PermissionFilter filter, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Permission?>> GetAll(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Permission?> GetById(PermissionId id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Update(Permission entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
