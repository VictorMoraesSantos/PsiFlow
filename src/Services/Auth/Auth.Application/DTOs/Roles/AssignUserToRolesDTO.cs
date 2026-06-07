namespace Auth.Application.DTOs.Roles
{
    public record AssignUserToRolesDTO(string UserId, IEnumerable<string> Roles);
}