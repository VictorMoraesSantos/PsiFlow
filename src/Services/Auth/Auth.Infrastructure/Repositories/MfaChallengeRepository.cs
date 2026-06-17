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
        public async Task<MfaChallenge?> GetById(MfaChallengeId id, CancellationToken cancellationToken = default)
        {
            var challenge = await dbContext.MfaChallenges.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return challenge;
        }

        public async Task<IEnumerable<MfaChallenge?>> GetAll(CancellationToken cancellationToken = default)
        {
            var challenges = await dbContext.MfaChallenges.AsNoTracking().ToListAsync(cancellationToken);
            return challenges;
        }

        public async Task<IEnumerable<MfaChallenge?>> Find(Expression<Func<MfaChallenge, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var challenges = await dbContext.MfaChallenges.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
            return challenges;
        }

        public async Task Create(MfaChallenge entity, CancellationToken cancellationToken = default)
        {
            await dbContext.MfaChallenges.AddAsync(entity, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<MfaChallenge> entities, CancellationToken cancellationToken = default)
        {
            await dbContext.MfaChallenges.AddRangeAsync(entities, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(MfaChallenge entity, CancellationToken cancellationToken = default)
        {
            dbContext.MfaChallenges.Update(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(MfaChallenge entity, CancellationToken cancellationToken = default)
        {
            dbContext.MfaChallenges.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<MfaChallenge?> GetActiveByUser(int userId, CancellationToken cancellationToken = default)
        {
            var active = await dbContext.MfaChallenges
                .Where(x => x.UserId == userId && !x.IsConfirmed && x.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
            return active;
        }
    }
}
