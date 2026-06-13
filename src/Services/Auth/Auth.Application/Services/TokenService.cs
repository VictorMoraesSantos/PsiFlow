using Auth.Application.Contracts;
using Auth.Application.Settings;
using BuildingBlocks.Results;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Auth.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _settings;
        private readonly JwtRsaKeyProvider _keyProvider;

        public TokenService(JwtSettings settings, JwtRsaKeyProvider keyProvider)
        {
            _settings = settings;
            _keyProvider = keyProvider;
        }

        public Result<string> GenerateToken(int userId, string email, bool emailVerified, int tenantId, string role, IEnumerable<string> roles, IEnumerable<string> permissions)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, email),
                    new Claim("email_verified", emailVerified ? "true" : "false", ClaimValueTypes.Boolean),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("roles", string.Join(",", roles))
                };

                if (tenantId > 0)
                {
                    claims.Add(new Claim("tenant_id", tenantId.ToString()));
                }

                foreach (var permission in permissions ?? Array.Empty<string>())
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

                return Result.Success(new JwtSecurityTokenHandler().WriteToken(token));
            }
            catch (Exception ex)
            {
                return Result.Failure<string>(Error.Failure($"Falha ao gerar token: {ex.Message}"));
            }
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
        }

        public string HashToken(string token)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(bytes);
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
                    return Result.Failure<ClaimsPrincipal>(Error.Failure("Algoritmo invalido"));
                return Result.Success(principal);
            }
            catch (Exception ex)
            {
                return Result.Failure<ClaimsPrincipal>(Error.Failure($"Falha ao validar token: {ex.Message}"));
            }
        }
    }
}
