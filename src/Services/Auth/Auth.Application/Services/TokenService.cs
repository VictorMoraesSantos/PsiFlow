using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using Auth.Application.DTOs.Token;
using Auth.Application.DTOs.Users;
using Auth.Application.Settings;
using Auth.Domain.Entities;
using Auth.Domain.Errors;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using BuildingBlocks.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Auth.Application.Services
{
    public class TokenService : ITokenService
    {
        private const int RefreshTokenLifetimeDays = 7;

        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly UserManager<User> _userManager;
        private readonly JwtSettings _settings;
        private readonly JwtRsaKeyProvider _keyProvider;

        public TokenService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            UserManager<User> userManager,
            JwtSettings settings,
            JwtRsaKeyProvider keyProvider)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _userManager = userManager;
            _settings = settings;
            _keyProvider = keyProvider;
        }

        public Result<string> GenerateToken(GenerateTokenDTO dto)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, dto.UserId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, dto.Email),
                    new Claim("email_verified", dto.EmailVerified ? "true" : "false", ClaimValueTypes.Boolean),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, dto.Role),
                    new Claim("roles", string.Join(",", dto.Roles))
                };

                if (dto.TenantId > 0)
                {
                    claims.Add(new Claim("tenant_id", dto.TenantId.ToString()));
                }

                foreach (var permission in dto.Permissions ?? Array.Empty<string>())
                {
                    if (string.IsNullOrWhiteSpace(permission)) continue;
                    claims.Add(new Claim("permission", permission));
                }

                var creds = new SigningCredentials(_keyProvider.SigningKey, SecurityAlgorithms.RsaSha256);
                var expires = DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes <= 0 ? 15 : _settings.ExpiryMinutes);

                var token = new JwtSecurityToken(
                    issuer: _settings.Issuer,
                    audience: _settings.Audience,
                    claims: claims,
                    notBefore: DateTime.UtcNow,
                    expires: expires,
                    signingCredentials: creds);

                var written = new JwtSecurityTokenHandler().WriteToken(token);
                var result = Result.Success(written);
                return result;
            }
            catch (Exception ex)
            {
                var error = Error.Failure($"Falha ao gerar token: {ex.Message}");
                var result = Result.Failure<string>(error);
                return result;
            }
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var token = Convert.ToBase64String(randomNumber)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
            return token;
        }

        public string HashToken(string token)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
            var hash = Convert.ToHexString(bytes);
            return hash;
        }

        public Result<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var validation = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _settings.Issuer,
                    ValidAudience = _settings.Audience,
                    IssuerSigningKey = _keyProvider.SigningKey
                };
                var principal = new JwtSecurityTokenHandler().ValidateToken(token, validation, out var securityToken);
                if (securityToken is not JwtSecurityToken jwt || !jwt.Header.Alg.Equals(SecurityAlgorithms.RsaSha256, StringComparison.OrdinalIgnoreCase))
                {
                    var result = Result.Failure<ClaimsPrincipal>(Error.Failure("Algoritmo invalido"));
                    return result;
                }
                var success = Result.Success(principal);
                return success;
            }
            catch (Exception ex)
            {
                var result = Result.Failure<ClaimsPrincipal>(Error.Failure($"Falha ao validar token: {ex.Message}"));
                return result;
            }
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
            var tokenResult = GenerateToken(tokenDTO);
            if (!tokenResult.IsSuccess)
            {
                var failure = Result.Failure<TokenResponse>(tokenResult.Error!);
                return failure;
            }

            var now = DateTime.UtcNow;
            var rawRefresh = GenerateRefreshToken();
            var refreshHash = HashToken(rawRefresh);
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

            var hash = HashToken(refreshToken);
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

        public async Task<Result> RevokeAllForUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetById(new UserId(userId), cancellationToken);
            if (user is null)
            {
                var failure = Result.Failure(UserErrors.NotFound(userId));
                return failure;
            }

            var active = await _refreshTokenRepository.ListActiveByUserAsync(userId, cancellationToken);
            var now = DateTime.UtcNow;
            foreach (var token in active) token.Revoke(now, revokedByIp: null, replacedByTokenId: null);
            await _refreshTokenRepository.UpdateRange(active, cancellationToken);

            var success = Result.Success();
            return success;
        }

        public async Task<Result> RevokeFamilyAsync(RefreshToken reusedToken, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var userTokens = await _refreshTokenRepository.ListActiveByUserAsync(reusedToken.UserId.Value, cancellationToken);
            foreach (var token in userTokens) token.Revoke(now, revokedByIp: null, replacedByTokenId: null);
            await _refreshTokenRepository.UpdateRange(userTokens, cancellationToken);

            var success = Result.Success();
            return success;
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
            var tokenResult = GenerateToken(tokenDTO);
            if (!tokenResult.IsSuccess)
            {
                var failure = Result.Failure<TokenResponse>(tokenResult.Error!);
                return failure;
            }

            var now = DateTime.UtcNow;
            var rawRefresh = GenerateRefreshToken();
            var refreshHash = HashToken(rawRefresh);
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
    }
}
