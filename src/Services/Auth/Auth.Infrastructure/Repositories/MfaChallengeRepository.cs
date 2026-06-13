using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using Auth.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Auth.Infrastructure.Repositories
{
    public class MfaChallengeRepository(ApplicationDbContext dbContext) : IMfaChallengeRepository
    {
        public async Task<MfaChallenge?> GetById(MfaChallengeId id, CancellationToken cancellationToken = default) =>
            await dbContext.MfaChallenges.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<IEnumerable<MfaChallenge?>> GetAll(CancellationToken cancellationToken = default) =>
            await dbContext.MfaChallenges.AsNoTracking().ToListAsync(cancellationToken);

        public async Task<IEnumerable<MfaChallenge?>> Find(Expression<Func<MfaChallenge, bool>> predicate, CancellationToken cancellationToken = default) =>
            await dbContext.MfaChallenges.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

        public async Task Create(MfaChallenge entity, CancellationToken cancellationToken = default) =>
            await dbContext.MfaChallenges.AddAsync(entity, cancellationToken);

        public async Task CreateRange(IEnumerable<MfaChallenge> entities, CancellationToken cancellationToken = default) =>
            await dbContext.MfaChallenges.AddRangeAsync(entities, cancellationToken);

        public async Task Update(MfaChallenge entity, CancellationToken cancellationToken = default) =>
            dbContext.MfaChallenges.Update(entity);

        public async Task Delete(MfaChallenge entity, CancellationToken cancellationToken = default) =>
            dbContext.MfaChallenges.Remove(entity);

        public async Task<MfaChallenge?> GetActiveByUser(int userId, CancellationToken cancellationToken = default) =>
            await dbContext.MfaChallenges
                .Where(x => x.UserId == userId && !x.IsConfirmed && x.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            await dbContext.SaveChangesAsync(cancellationToken);
    }
}
