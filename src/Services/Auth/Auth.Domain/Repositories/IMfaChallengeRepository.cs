using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Core.Domain.Repositories;

namespace Auth.Domain.Repositories
{
    public interface IMfaChallengeRepository : IRepository<MfaChallenge, MfaChallengeId>
    {
        Task<MfaChallenge?> GetActiveByUser(int userId, CancellationToken cancellationToken = default);
    }
}
