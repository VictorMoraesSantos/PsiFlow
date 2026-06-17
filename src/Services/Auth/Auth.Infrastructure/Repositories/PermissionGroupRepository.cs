using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using Auth.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Auth.Infrastructure.Repositories
{
    public class PermissionGroupRepository(ApplicationDbContext dbContext) : IPermissionGroupRepository
    {
        public async Task<PermissionGroup?> GetById(PermissionGroupId id, CancellationToken cancellationToken = default)
        {
            var group = await dbContext.PermissionGroups.Include(x => x.Permissions).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return group;
        }

        public async Task<IEnumerable<PermissionGroup?>> GetAll(CancellationToken cancellationToken = default)
        {
            var groups = await dbContext.PermissionGroups.AsNoTracking().ToListAsync(cancellationToken);
            return groups;
        }

        public async Task<IEnumerable<PermissionGroup?>> Find(Expression<Func<PermissionGroup, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var groups = await dbContext.PermissionGroups.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
            return groups;
        }

        public async Task Create(PermissionGroup entity, CancellationToken cancellationToken = default)
        {
            await dbContext.PermissionGroups.AddAsync(entity, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<PermissionGroup> entities, CancellationToken cancellationToken = default)
        {
            await dbContext.PermissionGroups.AddRangeAsync(entities, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(PermissionGroup entity, CancellationToken cancellationToken = default)
        {
            dbContext.PermissionGroups.Update(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(PermissionGroup entity, CancellationToken cancellationToken = default)
        {
            dbContext.PermissionGroups.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<PermissionGroup?> FindByKey(string groupKey, CancellationToken cancellationToken = default)
        {
            var group = await dbContext.PermissionGroups.FirstOrDefaultAsync(x => x.GroupKey == groupKey, cancellationToken);
            return group;
        }
    }
}
