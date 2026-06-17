using Auth.Application.DTOs.Users;
using Auth.Domain.Entities;

namespace Auth.Application.Mapping
{
    public static class UserMapper
    {
        public static UserSummaryDTO ToSummary(this User entity)
        {
            var dto = new UserSummaryDTO(entity.Id, entity.Name.FullName, entity.Email ?? string.Empty, entity.Role);
            return dto;

        }

        public static UserDTO ToDTO(this User entity)
        {
            var dto = new UserDTO(entity.Id, entity.CreatedAt, entity.UpdatedAt, entity.Name.FullName, entity.Email ?? string.Empty, entity.Role, entity.TenantId, entity.IsActive, entity.LastLoginAt);
            return dto;
        }
    }
}
