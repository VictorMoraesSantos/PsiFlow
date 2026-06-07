namespace Auth.Application.DTOs.Permissions
{
    public record RolePermissionsDTO(string RoleName, IEnumerable<PermissionDTO> Permissions);
}