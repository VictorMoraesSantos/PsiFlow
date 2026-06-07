namespace Auth.Application.DTOs.Permissions
{
    public record UserPermissionsDTO(int UserId, PermissionsClaimDTO Permissions);
}