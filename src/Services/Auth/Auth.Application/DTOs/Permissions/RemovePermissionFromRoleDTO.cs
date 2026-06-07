namespace Auth.Application.DTOs.Permissions
{
    public record RemovePermissionFromRoleDTO(string RoleName, string ClaimType, string ClaimValue);
}