namespace Auth.Application.DTOs.Auth
{
    public record RegisterResult(int UserId, int TenantId, string Email, string Role);
}
