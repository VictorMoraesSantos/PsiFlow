using Auth.Application.DTOs.Users;

namespace Auth.Application.DTOs.Auth
{
    public record TokenResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt, UserSummaryDTO User);
}
