namespace Auth.Application.DTOs.Users
{
    public record UserSummaryResponse(int Id, string Email, string FullName, string Role, int TenantId, bool IsActive);
}
