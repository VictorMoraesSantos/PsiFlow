using Auth.Domain.Entities;
using BuildingBlocks.Results;

namespace Auth.Application.Contracts
{
    public interface ITokenRevocationService
    {
        Task<Result> RevokeAllForUserAsync(int userId, CancellationToken cancellationToken = default);

        Task<Result> RevokeFamilyAsync(RefreshToken reusedToken, CancellationToken cancellationToken = default);
    }
}
