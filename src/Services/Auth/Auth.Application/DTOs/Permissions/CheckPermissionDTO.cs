namespace Auth.Application.DTOs.Permissions
{
    public record CheckPermissionDTO(int UserId, string ClaimType, string ClaimValue);
}