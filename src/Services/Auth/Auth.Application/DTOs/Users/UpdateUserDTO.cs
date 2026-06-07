namespace Auth.Application.DTOs.Users
{
    public record UpdateUserDTO(
        int Id,
        string FirstName,
        string LastName,
        string Email,
        DateOnly? BirthDate);
}