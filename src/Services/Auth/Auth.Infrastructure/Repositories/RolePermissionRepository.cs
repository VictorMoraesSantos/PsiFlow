using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Auth.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Auth.Infrastructure.Repositories
{
    public sealed class RolePermissionRepository(ApplicationDbContext dbContext) : IRolePermissionRepository
    {
        public async Task<RolePermission?> GetById(int id, CancellationToken cancellationToken = default) =>
            await dbContext.RolePermissions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<IEnumerable<RolePermission?>> GetAll(CancellationToken cancellationToken = default) =>
            await dbContext.RolePermissions.AsNoTracking().ToListAsync(cancellationToken);

        public async Task<IEnumerable<RolePermission?>> Find(Expression<Func<RolePermission, bool>> predicate, CancellationToken cancellationToken = default) =>
            await dbContext.RolePermissions.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

        public async Task Create(RolePermission entity, CancellationToken cancellationToken = default)
        {
            await dbContext.RolePermissions.AddAsync(entity, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<RolePermission> entities, CancellationToken cancellationToken = default)
        {
            await dbContext.RolePermissions.AddRangeAsync(entities, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(RolePermission entity, CancellationToken cancellationToken = default)
        {
            var entry = dbContext.Entry(entity);
            if (entry.State == EntityState.Detached) dbContext.RolePermissions.Attach(entity);
            entry.State = EntityState.Modified;
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(RolePermission entity, CancellationToken cancellationToken = default)
        {
            var entry = dbContext.Entry(entity);
            if (entry.State == EntityState.Detached) dbContext.RolePermissions.Attach(entity);
            entry.State = EntityState.Deleted;
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<string>> GetPermissionsForRoleAsync(string role, CancellationToken cancellationToken = default) =>
            await dbContext.RolePermissions.Where(x => x.Role == role).Select(x => x.Permission).ToListAsync(cancellationToken);
    }
}
