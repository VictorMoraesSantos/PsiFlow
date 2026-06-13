namespace Auth.Application.DTOs.Auth
{
    public record MfaSetupResult(string Secret, string QrCodeUri);
}
