namespace Auth.Application.DTOs.Auth
{
    public record SignUpDTO(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string ConfirmPassword);
}