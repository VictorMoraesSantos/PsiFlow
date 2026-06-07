namespace Auth.Application.DTOs.Auth
{
    public record ChangePasswordDTO(
        string CurrentPassword,
        string NewPassword,
        string ConfirmNewPassword);
}