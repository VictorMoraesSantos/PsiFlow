using Auth.Application.DTOs.Users;

namespace Auth.Application.DTOs.Auth
{
    public record RegisterRequest(
        string Role,
        string FullName,
        string Email,
        string Password,
        string ConfirmPassword,
        string? Phone,
        string? Crp,
        string AcceptedTermsVersion,
        string AcceptedPrivacyVersion);

    public record RegisterResult(int UserId, int TenantId, string Email, string Role);

    public record LoginRequest(string Email, string Password);
    public record RefreshRequest(string RefreshToken);
    public record LogoutRequest(string RefreshToken);
    public record MeResponse(int UserId, int TenantId, string Email, string Role, string FullName, bool IsActive);

    public record ForgotPasswordRequest(string Email);
    public record ResetPasswordRequest(string Email, string Token, string NewPassword, string ConfirmPassword);
    public record ChangePasswordRequest(string CurrentPassword, string NewPassword, string ConfirmNewPassword);
    public record ConsentRequest(string TermsVersion, string PrivacyVersion);

    public record MfaSetupResult(string Secret, string QrCodeUri);
    public record MfaVerifyRequest(string Code);

    public record TokenResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt, UserSummaryDTO User);
}
