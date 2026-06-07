namespace Auth.Application.DTOs.Roles
{
    public record UpdateUserRolesDTO(string UserId, IEnumerable<string> Roles);
}