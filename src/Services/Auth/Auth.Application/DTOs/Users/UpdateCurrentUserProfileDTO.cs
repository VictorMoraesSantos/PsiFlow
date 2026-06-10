namespace Auth.Application.DTOs.Users
{
    public record UpdateCurrentUserProfileDTO(string FullName, string Email, string? Phone);
}
