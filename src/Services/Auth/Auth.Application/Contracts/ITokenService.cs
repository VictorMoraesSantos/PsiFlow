using Auth.Application.DTOs.Token;
using BuildingBlocks.Results;

namespace Auth.Application.Contracts
{
    public interface ITokenService
    {
        Result GenerateToken(GenerateTokenDTO dto);
        Result GenerateRefreshToken();
        Result GetPrincipalFromExpiredToken(string token);
    }
}