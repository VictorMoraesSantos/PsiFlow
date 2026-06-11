using Auth.Application.DTOs.Users;
using Auth.Domain.Filters;
using BuildingBlocks.Results;

namespace Auth.Application.Contracts
{
    public interface IUserService
    {
        Task<Result<UserSummaryDTO>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<UserSummaryDTO>> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<UserSummaryDTO>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Result<(IEnumerable<UserSummaryDTO> Items, int TotalCount)>> GetByFilterAsync(UserFilter filter, CancellationToken cancellationToken = default);
        Task<Result> UpdateCurrentUserProfileAsync(int userId, UpdateCurrentUserProfileDTO dto, CancellationToken cancellationToken = default);
        Task<Result> DeleteCurrentUserAsync(int userId, CancellationToken cancellationToken = default);
    }
}
