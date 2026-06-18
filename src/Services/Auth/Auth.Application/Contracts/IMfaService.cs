using Auth.Application.DTOs.Auth;
using Auth.Domain.Entities;
using BuildingBlocks.Results;

namespace Auth.Application.Contracts
{
    public interface IMfaService
    {
        Task<Result<MfaChallengeStart>> StartLoginChallengeAsync(User user, CancellationToken cancellationToken = default);
        Task<Result<CompletedMfaChallenge>> CompleteLoginChallengeAsync(string mfaToken, string code, CancellationToken cancellationToken = default);
        Task<Result<MfaSetupResult>> SetupAsync(int userId, CancellationToken cancellationToken = default);
        Task<Result> VerifyAsync(int userId, string code, CancellationToken cancellationToken = default);
    }

    public sealed record MfaChallengeStart(int ChallengeId, string MfaToken);
    public sealed record CompletedMfaChallenge(User User);
}
