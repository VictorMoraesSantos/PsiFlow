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
        public TokenService(JwtSettings settings)
        {
            _settings = settings;
        }

        public Result<string> GenerateToken(int userId, string email, int tenantId, string role, IEnumerable<string> roles, IEnumerable<string> permissions)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("tenant_id", tenantId.ToString()),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("roles", string.Join(",", roles))
                };

                foreach (var permission in permissions ?? Array.Empty<string>())
                {
                    if (string.IsNullOrWhiteSpace(permission)) continue;
                    claims.Add(new Claim("permission", permission));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
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
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
                var validation = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _settings.Issuer,
                    ValidAudience = _settings.Audience,
                    IssuerSigningKey = key
                };
                var principal = new JwtSecurityTokenHandler().ValidateToken(token, validation, out var securityToken);
                if (securityToken is not JwtSecurityToken jwt || !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
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
