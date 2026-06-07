namespace Auth.Application.DTOs.Permissions
{
    public record AddPermissionToUserDTO(int UserId, string ClaimType, string ClaimValue);
}