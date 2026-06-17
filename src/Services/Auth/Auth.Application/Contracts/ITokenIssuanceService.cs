using Auth.Application.DTOs.Auth;
using Auth.Domain.Entities;
using BuildingBlocks.Results;

namespace Auth.Application.Contracts
{
    public interface ITokenIssuanceService
    {
        Task<Result<TokenResponse>> IssueAsync(User user, CancellationToken cancellationToken = default);

        Task<Result<TokenResponse>> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);
    }
}
