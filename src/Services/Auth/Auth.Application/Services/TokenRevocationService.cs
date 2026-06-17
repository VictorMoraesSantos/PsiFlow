using Auth.Application.Contracts;
using Auth.Domain.Entities;
using Auth.Domain.Errors;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using BuildingBlocks.Results;

namespace Auth.Application.Services
{
    public class TokenRevocationService : ITokenRevocationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public TokenRevocationService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<Result> RevokeAllForUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetById(new UserId(userId), cancellationToken);
            if (user is null) return Result.Failure(UserErrors.NotFound(userId));

            var active = await _refreshTokenRepository.ListActiveByUserAsync(userId, cancellationToken);
            var now = DateTime.UtcNow;
            foreach (var token in active) token.Revoke(now, revokedByIp: null, replacedByTokenId: null);
            await _refreshTokenRepository.UpdateRange(active, cancellationToken);
            return Result.Success();
        }

        public async Task<Result> RevokeFamilyAsync(RefreshToken reusedToken, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var userTokens = await _refreshTokenRepository.ListActiveByUserAsync(reusedToken.UserId.Value, cancellationToken);
            foreach (var token in userTokens) token.Revoke(now, revokedByIp: null, replacedByTokenId: null);
            await _refreshTokenRepository.UpdateRange(userTokens, cancellationToken);
            return Result.Success();
        }
    }
}
