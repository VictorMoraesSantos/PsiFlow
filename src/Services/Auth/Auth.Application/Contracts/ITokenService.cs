using Auth.Application.DTOs.Auth;
using Auth.Application.DTOs.Token;
using Auth.Domain.Entities;
using BuildingBlocks.Results;
using System.Security.Claims;

namespace Auth.Application.Contracts
{
    public interface ITokenService
    {
        Task<Result<TokenResponse>> IssueAsync(User user, CancellationToken cancellationToken = default);
        Task<Result<TokenResponse>> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<Result> RevokeAllForUserAsync(int userId, CancellationToken cancellationToken = default);
        Task<Result> RevokeFamilyAsync(RefreshToken reusedToken, CancellationToken cancellationToken = default);
        Result<string> GenerateToken(GenerateTokenDTO dto);
        string GenerateRefreshToken();
        string HashToken(string token);
        Result<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token);
    }
}
