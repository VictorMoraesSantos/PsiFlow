namespace Auth.Application.DTOs.Auth
{
    public record ExternalLoginDTO(
        string Email,
        string FirstName,
        string LastName,
        string Provider,
        string ProviderKey);
}