namespace Auth.Application.DTOs.Users
{
    public record UserDTO(int Id, string FullName, string Email, string Role, int TenantId, bool IsActive, DateTime CreatedAt, DateTime? LastLoginAt);
}
