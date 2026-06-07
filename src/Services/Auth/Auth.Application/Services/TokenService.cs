using Auth.Application.Contracts;
using Auth.Application.DTOs.Token;
using BuildingBlocks.Results;

namespace Auth.Application.Services
{
    public class TokenService : ITokenService
    {
        public Result GenerateRefreshToken()
        {
            throw new NotImplementedException();
        }

        public Result GenerateToken(GenerateTokenDTO dto)
        {
            throw new NotImplementedException();
        }

        public Result GetPrincipalFromExpiredToken(string token)
        {
            throw new NotImplementedException();
        }
    }
}
