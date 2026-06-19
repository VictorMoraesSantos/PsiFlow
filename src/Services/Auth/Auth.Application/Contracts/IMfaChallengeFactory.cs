using Auth.Domain.Entities;

namespace Auth.Application.Contracts
{
    public interface IMfaChallengeFactory
    {
        Task<MfaChallenge> CreateLoginChallengeAsync(User user, CancellationToken cancellationToken = default);
        Task<MfaChallenge> SetupChallengeAsync(User user, string secretBase32, string qrCodeUri, TimeSpan lifetime, CancellationToken cancellationToken = default);
        string DecryptSecret(MfaChallenge challenge);
    }
}
