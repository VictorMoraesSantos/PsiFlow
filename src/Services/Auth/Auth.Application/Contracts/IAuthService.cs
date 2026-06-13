using Auth.Application.DTOs.Auth;
using BuildingBlocks.Results;

namespace Auth.Application.Contracts
{
    public interface IAuthService
    {
        Task<Result<RegisterResult>> RegisterAsync(RegisterDTO dto, CancellationToken cancellationToken = default);
        Task<Result<TokenResponse>> LoginAsync(LoginDTO dto, CancellationToken cancellationToken = default);
        Task<Result<TokenResponse>> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<Result> LogoutAsync(int userId, CancellationToken cancellationToken = default);
        Task<Result<MeResponse>> MeAsync(int userId, CancellationToken cancellationToken = default);
        Task<Result> RecordConsentAsync(int userId, ConsentDTO dto, CancellationToken cancellationToken = default);
        Task<Result> ChangePasswordAsync(int userId, ChangePasswordDTO dto, CancellationToken cancellationToken = default);
        Task<Result> ForgotPasswordAsync(ForgotPasswordDTO dto, CancellationToken cancellationToken = default);
        Task<Result> ResetPasswordAsync(ResetPasswordDTO dto, CancellationToken cancellationToken = default);
        Task<Result<MfaSetupResult>> SetupMfaAsync(int userId, CancellationToken cancellationToken = default);
        Task<Result> VerifyMfaAsync(int userId, MfaVerifyDTO dto, CancellationToken cancellationToken = default);
    }
}
