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
    public class PermissionGroupRepository(ApplicationDbContext _dbContext) : IPermissionGroupRepository
    {
        public async Task<PermissionGroup?> GetById(PermissionGroupId id, CancellationToken cancellationToken = default)
        {
            var entity = await _dbContext.PermissionGroups.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return entity;
        }

        public async Task<IEnumerable<PermissionGroup?>> GetAll(CancellationToken cancellationToken = default)
        {
            var entities = await _dbContext.PermissionGroups.AsNoTracking().ToListAsync(cancellationToken);
            return entities;
        }

        public async Task<IEnumerable<PermissionGroup?>> Find(Expression<Func<PermissionGroup, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entities = await _dbContext.PermissionGroups.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
            return entities;
        }

        public async Task<(IEnumerable<PermissionGroup> Items, int TotalCount)> FindByFilter(PermissionGroupFilter filter, CancellationToken cancellationToken = default)
        {
            var spec = new PermissionGroupSpecification(filter);
            IQueryable<PermissionGroup> query = _dbContext.PermissionGroups.AsNoTracking();
            IQueryable<PermissionGroup> countQuery = spec.Criteria != null ? query.Where(spec.Criteria) : query;
            int totalCount = await countQuery.CountAsync(cancellationToken);
            IQueryable<PermissionGroup> finalQuery = SpecificationEvaluator.GetQuery(query, spec);
            IEnumerable<PermissionGroup> items = await finalQuery.ToListAsync(cancellationToken);
            return (items, totalCount);
        }

        public async Task Create(PermissionGroup entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.PermissionGroups.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<PermissionGroup> entities, CancellationToken cancellationToken = default)
        {
            await _dbContext.PermissionGroups.AddRangeAsync(entities, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(PermissionGroup entity, CancellationToken cancellationToken = default)
        {
            _dbContext.PermissionGroups.Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(PermissionGroup entity, CancellationToken cancellationToken = default)
        {
            _dbContext.PermissionGroups.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
