using Auth.Domain.Aggregates;
using Auth.Domain.Repositories;
using Auth.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Auth.Infrastructure.Repositories
{
    public class PermissionGroupRepository(ApplicationDbContext dbContext) : IPermissionGroupRepository
    {
        public async Task<PermissionGroup?> GetById(int id, CancellationToken cancellationToken = default) =>
            await dbContext.PermissionGroups.Include(x => x.Permissions).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<IEnumerable<PermissionGroup?>> GetAll(CancellationToken cancellationToken = default) =>
            await dbContext.PermissionGroups.AsNoTracking().ToListAsync(cancellationToken);

        public async Task<IEnumerable<PermissionGroup?>> Find(Expression<Func<PermissionGroup, bool>> predicate, CancellationToken cancellationToken = default) =>
            await dbContext.PermissionGroups.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

        public async Task Create(PermissionGroup entity, CancellationToken cancellationToken = default) =>
            await dbContext.PermissionGroups.AddAsync(entity, cancellationToken);

        public async Task CreateRange(IEnumerable<PermissionGroup> entities, CancellationToken cancellationToken = default) =>
            await dbContext.PermissionGroups.AddRangeAsync(entities, cancellationToken);

        public async Task Update(PermissionGroup entity, CancellationToken cancellationToken = default) =>
            dbContext.PermissionGroups.Update(entity);

        public async Task Delete(PermissionGroup entity, CancellationToken cancellationToken = default) =>
            dbContext.PermissionGroups.Remove(entity);

        public async Task<PermissionGroup?> FindByKeyAsync(string groupKey, CancellationToken cancellationToken = default) =>
            await dbContext.PermissionGroups.FirstOrDefaultAsync(x => x.GroupKey == groupKey, cancellationToken);

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            await dbContext.SaveChangesAsync(cancellationToken);
    }
}
