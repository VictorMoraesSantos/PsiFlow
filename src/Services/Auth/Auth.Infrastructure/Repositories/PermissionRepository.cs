using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using Auth.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Auth.Infrastructure.Repositories
{
    public class PermissionRepository(ApplicationDbContext dbContext) : IPermissionRepository
    {
        public async Task<Permission?> GetById(PermissionId id, CancellationToken cancellationToken = default) =>
            await dbContext.Permissions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<IEnumerable<Permission?>> GetAll(CancellationToken cancellationToken = default) =>
            await dbContext.Permissions.AsNoTracking().ToListAsync(cancellationToken);

        public async Task<IEnumerable<Permission?>> Find(Expression<Func<Permission, bool>> predicate, CancellationToken cancellationToken = default) =>
            await dbContext.Permissions.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

        public async Task Create(Permission entity, CancellationToken cancellationToken = default) =>
            await dbContext.Permissions.AddAsync(entity, cancellationToken);

        public async Task CreateRange(IEnumerable<Permission> entities, CancellationToken cancellationToken = default) =>
            await dbContext.Permissions.AddRangeAsync(entities, cancellationToken);

        public async Task Update(Permission entity, CancellationToken cancellationToken = default) =>
            dbContext.Permissions.Update(entity);

        public async Task Delete(Permission entity, CancellationToken cancellationToken = default) =>
            dbContext.Permissions.Remove(entity);

        public async Task<Permission?> FindByClaim(string claimType, string claimValue, CancellationToken cancellationToken = default) =>
            await dbContext.Permissions.FirstOrDefaultAsync(x => x.ClaimType == claimType && x.ClaimValue == claimValue, cancellationToken);

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            await dbContext.SaveChangesAsync(cancellationToken);
    }
}
