using Auth.Application.DTOs.Users;
using Auth.Domain.Entities;

namespace Auth.Application.Mapping
{
    public static class UserMapper
    {
        public static UserSummaryDTO ToSummary(this User user) =>
            new(user.Id, user.Name.FullName, user.Email ?? string.Empty, user.Role);

        public static UserDTO ToDTO(this User user) =>
            new UserDTO(user.Id, user.CreatedAt, user.UpdatedAt, user.Name.FullName, user.Email ?? string.Empty, user.Role, user.TenantId, user.IsActive, user.LastLoginAt);
    }
}
