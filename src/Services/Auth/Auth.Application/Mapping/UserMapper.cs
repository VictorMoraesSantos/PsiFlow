using Auth.Application.DTOs.Users;
using Auth.Domain.Aggregates;

namespace Auth.Application.Mapping
{
    public static class UserMapper
    {
        public static UserSummaryDTO ToSummary(this User user) =>
            new(user.Id, user.Name.FullName, user.Email ?? string.Empty, user.Role);
    }
}
