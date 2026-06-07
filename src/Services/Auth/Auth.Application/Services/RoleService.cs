using Auth.Application.Contracts;
using Auth.Application.DTOs.Roles;
using BuildingBlocks.Results;
using System.Security.Claims;

namespace Auth.Application.Services
{
    public class RoleService : IRoleService
    {
        public Task<Result> AssignUserToRolesAsync(AssignUserToRolesDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> CreateRoleAsync(CreateRoleDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteRoleAsync(DeleteRoleDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> GetCurrentUserRolesAsync(ClaimsPrincipal user)
        {
            throw new NotImplementedException();
        }

        public Task<Result> GetUserRolesAsync(GetUserRolesDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> RemoveUserFromRolesAsync(RemoveUserFromRolesDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UpdateRoleAsync(UpdateRoleDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UpdateUserRolesAsync(UpdateUserRolesDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
