namespace Auth.Application.DTOs.Auth
{
    public record ResetPasswordDTO(string Email, string Token, string NewPassword, string ConfirmPassword);
}
