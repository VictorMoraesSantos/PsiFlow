using Auth.Application.Contracts;
using Auth.Application.DTOs.Users;
using BuildingBlocks.Results;
using System.Security.Claims;

namespace Auth.Application.Services
{
    public class UserService : IUserService
    {
        public Task<Result> ChangeCurrentUserPasswordAsync(ClaimsPrincipal user, ChangeCurrentUserPasswordDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteCurrentUserAsync(ClaimsPrincipal user)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result> GetAllUsersDetailsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result> GetCurrentUserDetailsAsync(ClaimsPrincipal user)
        {
            throw new NotImplementedException();
        }

        public Task<Result> GetUserDetailsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> IsUserEmailUniqueAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UpdateCurrentUserProfileAsync(ClaimsPrincipal user, UpdateCurrentUserProfileDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UpdateUserProfileAsync(UpdateUserDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
