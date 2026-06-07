using Auth.Application.DTOs.Users;
using BuildingBlocks.Results;
using System.Security.Claims;

namespace Auth.Application.Contracts
{
    public interface IUserService
    {
        Task<Result> GetCurrentUserDetailsAsync(ClaimsPrincipal user);
        Task<Result> UpdateCurrentUserProfileAsync(ClaimsPrincipal user, UpdateCurrentUserProfileDTO dto);
        Task<Result> ChangeCurrentUserPasswordAsync(ClaimsPrincipal user, ChangeCurrentUserPasswordDTO dto);
        Task<Result> DeleteCurrentUserAsync(ClaimsPrincipal user);
        Task<Result> GetUserDetailsAsync(string userId);
        Task<Result> GetAllUsersAsync();
        Task<Result> GetAllUsersDetailsAsync();
        Task<Result> IsUserEmailUniqueAsync(string email);
        Task<Result> UpdateUserProfileAsync(UpdateUserDTO dto);
        Task<Result> DeleteUserAsync(string userId);
    }
}