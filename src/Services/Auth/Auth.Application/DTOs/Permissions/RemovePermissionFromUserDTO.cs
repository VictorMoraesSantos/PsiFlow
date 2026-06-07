namespace Auth.Application.DTOs.Permissions
{
    public record RemovePermissionFromUserDTO(int UserId, string ClaimType, string ClaimValue);
}