using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using BuildingBlocks.Results;

namespace Auth.Application.Services
{
    public class AuthService : IAuthService
    {
        public Task<Result> ChangePasswordAsync(ChangePasswordDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> ConfirmEmailAsync(ConfirmEmailDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> ExternalLoginAsync(ExternalLoginDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> LoginAsync(LoginDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> LogoutAsync(LogoutDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> RegisterAsync(SignUpDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> ResetPasswordAsync(ResetPasswordDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> RevokeRefreshTokenAsync(RevokeRefreshTokenDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> SendEmailConfirmationAsync(SendEmailConfirmationDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> SendPasswordResetAsync(SendPasswordResetDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UpdateRefreshTokenAsync(UpdateRefreshTokenDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
