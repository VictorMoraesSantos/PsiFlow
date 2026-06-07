using System.Security.Claims;

namespace Auth.Application.DTOs.Token
{
    public record GenerateTokenDTO(
        string UserId,
        string Email,
        IEnumerable<string> Roles,
        CancellationToken CancellationToken,
        IList<Claim>? Claims = null);
}