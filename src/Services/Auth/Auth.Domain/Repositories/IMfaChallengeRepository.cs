using Auth.Domain.Aggregates;
using Core.Domain.Repositories;

namespace Auth.Domain.Repositories
{
    public interface IMfaChallengeRepository : IRepository<MfaChallenge, int>
    {
        Task<MfaChallenge?> GetActiveByUserAsync(int userId, CancellationToken cancellationToken = default);
    }
}
