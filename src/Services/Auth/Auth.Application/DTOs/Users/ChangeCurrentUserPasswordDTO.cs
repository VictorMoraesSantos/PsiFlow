namespace Auth.Application.DTOs.Users
{
    public record ChangeCurrentUserPasswordDTO(
        string CurrentPassword,
        string NewPassword);
}