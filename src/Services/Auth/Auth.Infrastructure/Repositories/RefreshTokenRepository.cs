using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using Auth.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Auth.Infrastructure.Repositories
{
    public sealed class RefreshTokenRepository(ApplicationDbContext dbContext) : IRefreshTokenRepository
    {
        public async Task<RefreshToken?> GetById(RefreshTokenId id, CancellationToken cancellationToken = default) =>
            await dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<IEnumerable<RefreshToken?>> GetAll(CancellationToken cancellationToken = default) =>
            await dbContext.RefreshTokens.AsNoTracking().ToListAsync(cancellationToken);

        public async Task<IEnumerable<RefreshToken?>> Find(Expression<Func<RefreshToken, bool>> predicate, CancellationToken cancellationToken = default) =>
            await dbContext.RefreshTokens.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

        public async Task Create(RefreshToken entity, CancellationToken cancellationToken = default)
        {
            await dbContext.RefreshTokens.AddAsync(entity, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<RefreshToken> entities, CancellationToken cancellationToken = default)
        {
            await dbContext.RefreshTokens.AddRangeAsync(entities, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(RefreshToken entity, CancellationToken cancellationToken = default)
        {
            var entry = dbContext.Entry(entity);
            if (entry.State == EntityState.Detached) dbContext.RefreshTokens.Attach(entity);
            entry.State = EntityState.Modified;
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(RefreshToken entity, CancellationToken cancellationToken = default)
        {
            var entry = dbContext.Entry(entity);
            if (entry.State == EntityState.Detached) dbContext.RefreshTokens.Attach(entity);
            entry.State = EntityState.Deleted;
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default) =>
            dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

        public async Task<IReadOnlyList<RefreshToken>> ListActiveByUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            return await dbContext.RefreshTokens
                .Where(x => x.UserId == userId && x.RevokedAt == null && x.ExpiresAt > now)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateRange(IEnumerable<RefreshToken> tokens, CancellationToken cancellationToken = default)
        {
            foreach (var token in tokens)
            {
                var entry = dbContext.Entry(token);
                if (entry.State == EntityState.Detached) dbContext.RefreshTokens.Attach(token);
                entry.State = EntityState.Modified;
            }
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
