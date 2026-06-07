namespace Auth.Application.DTOs.Permissions
{
    public record PermissionsClaimDTO(Dictionary<string, IEnumerable<string>> Permissions);
}