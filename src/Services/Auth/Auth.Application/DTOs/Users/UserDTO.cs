using Core.Application.DTO;

namespace Auth.Application.DTOs.Users
{
    public record UserDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string FullName,
        string Email,
        string Role,
        int TenantId,
        bool IsActive,
        DateTime? LastLoginAt)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
