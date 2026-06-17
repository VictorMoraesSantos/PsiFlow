using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using Auth.Application.DTOs.Token;
using Auth.Application.DTOs.Users;
using Auth.Domain.Entities;
using Auth.Domain.Errors;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using BuildingBlocks.Results;
using Microsoft.AspNetCore.Identity;

namespace Auth.Application.Services
{
    public class TokenIssuanceService : ITokenIssuanceService
    {
        private const int RefreshTokenLifetimeDays = 7;

        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public TokenIssuanceService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            UserManager<User> userManager,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<Result<TokenResponse>> IssueAsync(User user, CancellationToken cancellationToken = default)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var permissionClaims = await _userManager.GetClaimsAsync(user);
            var permissionValues = permissionClaims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .ToList();

            var tokenDTO = new GenerateTokenDTO(
                user.Id.Value,
                user.Email ?? string.Empty,
                user.EmailConfirmed,
                user.TenantId.Value,
                user.Role,
                roles,
                permissionValues);
            var tokenResult = _tokenService.GenerateToken(tokenDTO);
            if (!tokenResult.IsSuccess)
            {
                var failure = Result.Failure<TokenResponse>(tokenResult.Error!);
                return failure;
            }

            var now = DateTime.UtcNow;
            var rawRefresh = _tokenService.GenerateRefreshToken();
            var refreshHash = _tokenService.HashToken(rawRefresh);
            var lifetime = TimeSpan.FromDays(RefreshTokenLifetimeDays);

            var issued = RefreshToken.Issue(user.Id, user.TenantId, refreshHash, now, lifetime, createdByIp: null, userAgent: null);
            await _refreshTokenRepository.Create(issued, cancellationToken);

            var summary = new UserSummaryDTO(user.Id.Value, user.Name.FullName, user.Email ?? string.Empty, user.Role);
            var response = new TokenResponse(
                tokenResult.Value!,
                rawRefresh,
                issued.ExpiresAt,
                summary);
            var success = Result.Success(response);
            return success;
        }

        public async Task<Result<TokenResponse>> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                var failure = Result.Failure<TokenResponse>(UserErrors.RefreshTokenInvalid);
                return failure;
            }

            var hash = _tokenService.HashToken(refreshToken);
            var existing = await _refreshTokenRepository.GetByHashAsync(hash, cancellationToken);
            if (existing is null)
            {
                var failure = Result.Failure<TokenResponse>(UserErrors.RefreshTokenInvalid);
                return failure;
            }
            if (!existing.BelongsTo(existing.UserId.Value))
            {
                var failure = Result.Failure<TokenResponse>(UserErrors.RefreshTokenInvalid);
                return failure;
            }

            if (existing.IsRevoked())
            {
                await RevokeFamilyAsync(existing, cancellationToken);
                var failure = Result.Failure<TokenResponse>(UserErrors.RefreshTokenReused);
                return failure;
            }

            if (existing.IsExpired(DateTime.UtcNow))
            {
                existing.Revoke(DateTime.UtcNow, revokedByIp: null, replacedByTokenId: null);
                await _refreshTokenRepository.Update(existing, cancellationToken);
                var failure = Result.Failure<TokenResponse>(UserErrors.RefreshTokenExpired);
                return failure;
            }

            var user = await _userRepository.GetById(new UserId(existing.UserId.Value), cancellationToken);
            if (user is null)
            {
                var failure = Result.Failure<TokenResponse>(UserErrors.NotFound(existing.UserId.Value));
                return failure;
            }

            var rotationResult = await RotateAsync(user, existing, cancellationToken);
            return rotationResult;
        }

        private async Task<Result<TokenResponse>> RotateAsync(User user, RefreshToken previous, CancellationToken cancellationToken)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var permissionClaims = await _userManager.GetClaimsAsync(user);
            var permissionValues = permissionClaims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .ToList();

            var tokenDTO = new GenerateTokenDTO(
                user.Id.Value,
                user.Email ?? string.Empty,
                user.EmailConfirmed,
                user.TenantId.Value,
                user.Role,
                roles,
                permissionValues);
            var tokenResult = _tokenService.GenerateToken(tokenDTO);
            if (!tokenResult.IsSuccess)
            {
                var failure = Result.Failure<TokenResponse>(tokenResult.Error!);
                return failure;
            }

            var now = DateTime.UtcNow;
            var rawRefresh = _tokenService.GenerateRefreshToken();
            var refreshHash = _tokenService.HashToken(rawRefresh);
            var lifetime = TimeSpan.FromDays(RefreshTokenLifetimeDays);

            var replacement = previous.Rotate(refreshHash, now, lifetime);
            await _refreshTokenRepository.Update(previous, cancellationToken);
            await _refreshTokenRepository.Create(replacement, cancellationToken);

            var summary = new UserSummaryDTO(user.Id.Value, user.Name.FullName, user.Email ?? string.Empty, user.Role);
            var response = new TokenResponse(
                tokenResult.Value!,
                rawRefresh,
                replacement.ExpiresAt,
                summary);
            var success = Result.Success(response);
            return success;
        }

        private async Task RevokeFamilyAsync(RefreshToken reused, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var userTokens = await _refreshTokenRepository.ListActiveByUserAsync(reused.UserId.Value, cancellationToken);
            foreach (var token in userTokens) token.Revoke(now, revokedByIp: null, replacedByTokenId: null);
            await _refreshTokenRepository.UpdateRange(userTokens, cancellationToken);
        }
    }
}
