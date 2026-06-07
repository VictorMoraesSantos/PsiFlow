namespace Auth.Application.DTOs.Roles
{
    public record RemoveUserFromRolesDTO(string UserId, IEnumerable<string> Roles);
}