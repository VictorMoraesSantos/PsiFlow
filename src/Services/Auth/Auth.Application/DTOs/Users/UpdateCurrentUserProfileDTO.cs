namespace Auth.Application.DTOs.Users
{
    public record UpdateCurrentUserProfileDTO(
        string FirstName,
        string LastName,
        string Email);
}