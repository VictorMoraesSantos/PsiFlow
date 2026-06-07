using Auth.Application.DTOs.Users;

namespace Auth.Application.DTOs.Auth
{
    public record AuthResult(string AccessToken, string RefreshToken, UserDTO User);
}