namespace Auth.Application.DTOs.Auth
{
    public record ConsentDTO(string DocumentType, string TermsVersion, string PrivacyVersion, string? IpAddress, string? UserAgent);
}
