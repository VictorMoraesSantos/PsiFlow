using Auth.Application.Contracts;
using Auth.Application.DTOs.Permissions;
using BuildingBlocks.Results;
using System.Security.Claims;

namespace Auth.Application.Services
{
    public class PermissionService : IPermissionService
    {
        public Task<Result> AddPermissionToRoleAsync(AddPermissionToRoleDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> AddPermissionToUserAsync(AddPermissionToUserDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> GetCurrentUserPermissionsAsync(ClaimsPrincipal user)
        {
            throw new NotImplementedException();
        }

        public Task<Result> GetRolePermissionsAsync(string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<Result> GetUserGroupsAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> HasPermissionAsync(CheckPermissionDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> RemovePermissionFromRoleAsync(RemovePermissionFromRoleDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> RemovePermissionFromUserAsync(RemovePermissionFromUserDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
