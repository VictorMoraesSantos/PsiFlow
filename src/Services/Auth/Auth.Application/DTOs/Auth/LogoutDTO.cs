using System.Security.Claims;

namespace Auth.Application.DTOs.Auth
{
    public record LogoutDTO(ClaimsPrincipal User);
}