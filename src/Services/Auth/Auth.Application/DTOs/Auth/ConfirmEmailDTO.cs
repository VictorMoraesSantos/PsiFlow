namespace Auth.Application.DTOs.Auth
{
    public record ConfirmEmailDTO(string UserId, string Token);
}