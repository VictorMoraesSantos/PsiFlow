using Auth.Domain.Aggregates;
using Auth.Domain.Filters;
using Auth.Domain.Filters.Specifications;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using Auth.Infrastructure.Persistence.Data;
using Core.Infrastructure.Persistence.Specifications;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Auth.Infrastructure.Repositories
{
    public class PermissionRepository(ApplicationDbContext _dbContext) : IPermissionRepository
    {
        public async Task<Permission?> GetById(PermissionId id, CancellationToken cancellationToken = default)
        {
            var entity = await _dbContext.Permissions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return entity;
        }

        public async Task<IEnumerable<Permission?>> GetAll(CancellationToken cancellationToken = default)
        {
            var entities = await _dbContext.Permissions.AsNoTracking().ToListAsync(cancellationToken);
            return entities;
        }

        public async Task<IEnumerable<Permission?>> Find(Expression<Func<Permission, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entities = await _dbContext.Permissions.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
            return entities;
        }

        public async Task<(IEnumerable<Permission> Items, int TotalCount)> FindByFilter(PermissionFilter filter, CancellationToken cancellationToken = default)
        {
            var spec = new PermissionSpecification(filter);
            IQueryable<Permission> query = _dbContext.Permissions.AsNoTracking();
            IQueryable<Permission> countQuery = spec.Criteria != null ? query.Where(spec.Criteria) : query;
            int totalCount = await countQuery.CountAsync(cancellationToken);
            IQueryable<Permission> finalQuery = SpecificationEvaluator.GetQuery(query, spec);
            IEnumerable<Permission> items = await finalQuery.ToListAsync(cancellationToken);
            return (items, totalCount);
        }

        public async Task Create(Permission entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.Permissions.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<Permission> entities, CancellationToken cancellationToken = default)
        {
            await _dbContext.Permissions.AddRangeAsync(entities, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(Permission entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Permissions.Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(Permission entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Permissions.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
