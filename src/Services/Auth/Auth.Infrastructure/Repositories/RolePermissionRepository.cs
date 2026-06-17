using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Auth.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Auth.Infrastructure.Repositories
{
    public sealed class RolePermissionRepository(ApplicationDbContext dbContext) : IRolePermissionRepository
    {
        public async Task<RolePermission?> GetById(int id, CancellationToken cancellationToken = default)
        {
            var rolePermission = await dbContext.RolePermissions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return rolePermission;
        }

        public async Task<IEnumerable<RolePermission?>> GetAll(CancellationToken cancellationToken = default)
        {
            var rolePermissions = await dbContext.RolePermissions.AsNoTracking().ToListAsync(cancellationToken);
            return rolePermissions;
        }

        public async Task<IEnumerable<RolePermission?>> Find(Expression<Func<RolePermission, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var rolePermissions = await dbContext.RolePermissions.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
            return rolePermissions;
        }

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
            dbContext.RolePermissions.Update(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(RolePermission entity, CancellationToken cancellationToken = default)
        {
            dbContext.RolePermissions.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<string>> GetPermissionsForRoleAsync(string role, CancellationToken cancellationToken = default)
        {
            var permissions = await dbContext.RolePermissions.Where(x => x.Role == role).Select(x => x.Permission).ToListAsync(cancellationToken);
            return permissions;
        }
    }
}
