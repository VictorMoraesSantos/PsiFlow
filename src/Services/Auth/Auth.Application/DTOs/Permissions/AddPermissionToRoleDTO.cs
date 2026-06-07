namespace Auth.Application.DTOs.Permissions
{
    public record AddPermissionToRoleDTO(string RoleName, string ClaimType, string ClaimValue);
}