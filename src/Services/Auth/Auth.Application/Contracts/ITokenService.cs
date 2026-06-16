using Auth.Application.DTOs.Token;
using BuildingBlocks.Results;
using System.Security.Claims;

namespace Auth.Application.Contracts
{
    public interface ITokenService
    {
        Result<string> GenerateToken(GenerateTokenDTO dto);
        string GenerateRefreshToken();
        string HashToken(string token);
        Result<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token);
    }
}
