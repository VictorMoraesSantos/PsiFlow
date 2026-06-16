namespace Auth.Application.DTOs.Auth
{
    public record MeResponse(
        int UserId,
        int TenantId,
        string Email,
        string Role,
        string FullName,
        bool IsActive,
        bool EmailVerified,
        bool MfaEnabled,
        IReadOnlyList<string> Permissions,
        IReadOnlyList<string> Roles);
}
