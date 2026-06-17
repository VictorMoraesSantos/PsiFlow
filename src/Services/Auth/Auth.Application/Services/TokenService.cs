using Auth.Application.Contracts;
using Auth.Application.DTOs.Token;
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
    }
}
