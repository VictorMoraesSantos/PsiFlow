namespace Auth.Application.DTOs.Permissions
{
    public record PermissionGroupDTO(string GroupKey, IEnumerable<PermissionDTO> Permissions);
}