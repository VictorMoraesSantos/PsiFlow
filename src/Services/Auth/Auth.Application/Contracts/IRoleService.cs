using Auth.Application.DTOs.Roles;
using BuildingBlocks.Results;
using System.Security.Claims;

namespace Auth.Application.Contracts
{
    public interface IRoleService
    {
        Task<Result> GetUserRolesAsync(GetUserRolesDTO dto);
        Task<Result> GetCurrentUserRolesAsync(ClaimsPrincipal user);
        Task<Result> CreateRoleAsync(CreateRoleDTO dto);
        Task<Result> UpdateRoleAsync(UpdateRoleDTO dto);
        Task<Result> DeleteRoleAsync(DeleteRoleDTO dto);
        Task<Result> AssignUserToRolesAsync(AssignUserToRolesDTO dto);
        Task<Result> RemoveUserFromRolesAsync(RemoveUserFromRolesDTO dto);
        Task<Result> UpdateUserRolesAsync(UpdateUserRolesDTO dto);
    }
}