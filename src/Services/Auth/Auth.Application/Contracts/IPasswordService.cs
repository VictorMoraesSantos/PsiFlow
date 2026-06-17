using Auth.Application.DTOs.Auth;
using BuildingBlocks.Results;

namespace Auth.Application.Contracts
{
    public interface IPasswordService
    {
        Task<Result> ChangeAsync(int userId, ChangePasswordDTO dto, CancellationToken cancellationToken = default);

        Task<Result> ForgotAsync(ForgotPasswordDTO dto, CancellationToken cancellationToken = default);

        Task<Result> ResetAsync(ResetPasswordDTO dto, CancellationToken cancellationToken = default);
    }
}
