using Auth.Application.DTOs.Auth;
using BuildingBlocks.Results;

namespace Auth.Application.Contracts
{
    public interface IAuthService
    {
        Task<Result<RegisterResult>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
        Task<Result<TokenResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
        Task<Result<TokenResponse>> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<Result> LogoutAsync(int userId, CancellationToken cancellationToken = default);
        Task<Result<MeResponse>> MeAsync(int userId, CancellationToken cancellationToken = default);
        Task<Result> RecordConsentAsync(int userId, ConsentRequest request, CancellationToken cancellationToken = default);
        Task<Result> ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken cancellationToken = default);
        Task<Result> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default);
        Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);
        Task<Result<MfaSetupResult>> SetupMfaAsync(int userId, CancellationToken cancellationToken = default);
        Task<Result> VerifyMfaAsync(int userId, MfaVerifyRequest request, CancellationToken cancellationToken = default);
    }
}
