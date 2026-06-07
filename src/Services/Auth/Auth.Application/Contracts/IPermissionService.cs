using Auth.Application.DTOs.Permissions;
using BuildingBlocks.Results;
using System.Security.Claims;

namespace Auth.Application.Contracts
{
    public interface IPermissionService
    {
        Task<Result> GetUserGroupsAsync(int userId);
        Task<Result> GetRolePermissionsAsync(string roleName);
        Task<Result> AddPermissionToUserAsync(AddPermissionToUserDTO dto);
        Task<Result> RemovePermissionFromUserAsync(RemovePermissionFromUserDTO dto);
        Task<Result> RemovePermissionFromRoleAsync(RemovePermissionFromRoleDTO dto);
        Task<Result> AddPermissionToRoleAsync(AddPermissionToRoleDTO dto);
        Task<Result> HasPermissionAsync(CheckPermissionDTO dto);
        Task<Result> GetCurrentUserPermissionsAsync(ClaimsPrincipal user);
    }
}