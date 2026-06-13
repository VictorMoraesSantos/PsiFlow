using BuildingBlocks.Results;
using System.Security.Claims;

namespace Auth.Application.Contracts
{
    public interface ITokenService
    {
        Result<string> GenerateToken(int userId, string email, int tenantId, string role, IEnumerable<string> roles, IEnumerable<string> permissions);
        string GenerateRefreshToken();
        string HashToken(string token);
        Result<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token);
    }
}
