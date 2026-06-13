using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Core.Domain.Repositories;

namespace Auth.Domain.Repositories
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken, RefreshTokenId>
    {
        Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<RefreshToken>> ListActiveByUserAsync(int userId, CancellationToken cancellationToken = default);
        Task UpdateRange(IEnumerable<RefreshToken> tokens, CancellationToken cancellationToken = default);
    }
}
