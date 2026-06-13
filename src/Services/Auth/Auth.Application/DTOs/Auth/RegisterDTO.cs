namespace Auth.Application.DTOs.Auth
{
    public record RegisterDTO(
        string Role,
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string ConfirmPassword,
        string? Phone,
        string? Crp,
        string AcceptedTermsVersion,
        string AcceptedPrivacyVersion);
}
