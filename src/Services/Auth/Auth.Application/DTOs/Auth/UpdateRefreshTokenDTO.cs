using System.Security.Claims;

namespace Auth.Application.DTOs.Auth
{
    public record UpdateRefreshTokenDTO(ClaimsPrincipal User, string RefreshToken);
}