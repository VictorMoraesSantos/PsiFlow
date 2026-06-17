using Auth.Application.DTOs.Auth;
using BuildingBlocks.Results;

namespace Auth.Application.Contracts
{
    public interface IUserProfileService
    {
        Task<Result<MeResponse>> GetMeAsync(int userId, CancellationToken cancellationToken = default);
    }
}
