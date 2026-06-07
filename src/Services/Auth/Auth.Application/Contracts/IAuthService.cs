using Auth.Application.DTOs.Auth;
using BuildingBlocks.Results;

namespace Auth.Application.Contracts
{
    public interface IAuthService
    {
        Task<Result> LoginAsync(LoginDTO dto);
        Task<Result> ExternalLoginAsync(ExternalLoginDTO dto);
        Task<Result> RegisterAsync(SignUpDTO dto);
        Task<Result> LogoutAsync(LogoutDTO dto);
        Task<Result> UpdateRefreshTokenAsync(UpdateRefreshTokenDTO dto);
        Task<Result> RevokeRefreshTokenAsync(RevokeRefreshTokenDTO dto);
        Task<Result> SendEmailConfirmationAsync(SendEmailConfirmationDTO dto);
        Task<Result> ConfirmEmailAsync(ConfirmEmailDTO dto);
        Task<Result> SendPasswordResetAsync(SendPasswordResetDTO dto);
        Task<Result> ResetPasswordAsync(ResetPasswordDTO dto);
        Task<Result> ChangePasswordAsync(ChangePasswordDTO dto);
    }
}