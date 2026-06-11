using Auth.Domain.Entities;
using Core.Domain.Repositories;

namespace Auth.Domain.Repositories
{
    public interface IMfaChallengeRepository : IRepository<MfaChallenge, int>
    {
        Task<MfaChallenge?> GetActiveByUserAsync(int userId, CancellationToken cancellationToken = default);
    }
}
