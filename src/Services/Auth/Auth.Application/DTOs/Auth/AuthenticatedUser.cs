using Auth.Domain.Entities;

namespace Auth.Application.DTOs.Auth
{
    public sealed record AuthenticatedUser(User User, bool RequiresMfa);
}
