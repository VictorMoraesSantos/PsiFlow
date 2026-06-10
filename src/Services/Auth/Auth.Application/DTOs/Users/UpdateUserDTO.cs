namespace Auth.Application.DTOs.Users
{
    public record UpdateUserDTO(int Id, string FullName, string Email, string? Phone);
}
