using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using Auth.Domain.Entities;
using Auth.Domain.Errors;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using BuildingBlocks.Results;
using Core.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Auth.Application.Services
{
    public class MfaService : IMfaService
    {
        private static readonly TimeSpan ChallengeLifetime = TimeSpan.FromMinutes(10);

        private readonly IUserRepository _userRepository;
        private readonly IMfaChallengeRepository _mfaChallengeRepository;
        private readonly IMfaChallengeFactory _mfaChallengeFactory;
        private readonly MfaLoginStore _mfaLoginStore;
        private readonly ILogger<MfaService> _logger;

        public MfaService(
            IUserRepository userRepository,
            IMfaChallengeRepository mfaChallengeRepository,
            IMfaChallengeFactory mfaChallengeFactory,
            MfaLoginStore mfaLoginStore,
            ILogger<MfaService> logger)
        {
            _userRepository = userRepository;
            _mfaChallengeRepository = mfaChallengeRepository;
            _mfaChallengeFactory = mfaChallengeFactory;
            _mfaLoginStore = mfaLoginStore;
            _logger = logger;
        }

        public async Task<Result<MfaChallengeStart>> StartLoginChallengeAsync(User user, CancellationToken cancellationToken = default)
        {
            var challenge = await _mfaChallengeFactory.CreateLoginChallengeAsync(user, cancellationToken);
            var mfaToken = _mfaLoginStore.Create(user.Id.Value, challenge.Id.Value);
            var challengeStart = new MfaChallengeStart(challenge.Id.Value, mfaToken);
            var result = Result.Success(challengeStart);
            return result;
        }

        public async Task<Result<CompletedMfaChallenge>> CompleteLoginChallengeAsync(string mfaToken, string code, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(mfaToken) || string.IsNullOrWhiteSpace(code))
            {
                var result = Result.Failure<CompletedMfaChallenge>(UserErrors.MfaCodeInvalid);
                return result;
            }
            if (!_mfaLoginStore.TryConsume(mfaToken, out var entry))
            {
                var result = Result.Failure<CompletedMfaChallenge>(UserErrors.MfaCodeInvalid);
                return result;
            }

            var user = await _userRepository.GetById(new UserId(entry.UserId), cancellationToken);
            if (user is null)
            {
                var result = Result.Failure<CompletedMfaChallenge>(UserErrors.NotFound(entry.UserId));
                return result;
            }

            var challenge = await _mfaChallengeRepository.GetById(new MfaChallengeId(entry.ChallengeId), cancellationToken);
            if (challenge is null)
            {
                var result = Result.Failure<CompletedMfaChallenge>(UserErrors.MfaChallengeNotFound);
                return result;
            }
            if (!challenge.BelongsTo(user.Id.Value))
            {
                var result = Result.Failure<CompletedMfaChallenge>(UserErrors.MfaCodeInvalid);
                return result;
            }
            if (!challenge.IsUsable(DateTime.UtcNow))
            {
                var result = Result.Failure<CompletedMfaChallenge>(UserErrors.MfaChallengeNotFound);
                return result;
            }

            try
            {
                var secret = _mfaChallengeFactory.DecryptSecret(challenge);
                challenge.Confirm(secret, code.Trim(), DateTime.UtcNow);
                await _mfaChallengeRepository.Update(challenge, cancellationToken);
            }
            catch (DomainException ex)
            {
                _logger.LogInformation("Falha ao confirmar challenge MFA: {Message}", ex.Message);
                var result = Result.Failure<CompletedMfaChallenge>(UserErrors.MfaCodeInvalid);
                return result;
            }

            var completed = new CompletedMfaChallenge(user);
            var success = Result.Success(completed);
            return success;
        }

        public async Task<Result<MfaSetupResult>> SetupAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetById(new UserId(userId), cancellationToken);
            if (user is null)
            {
                var result = Result.Failure<MfaSetupResult>(UserErrors.NotFound(userId));
                return result;
            }
            if (!user.IsMfaEligible())
            {
                var result = Result.Failure<MfaSetupResult>(UserErrors.MfaNotAllowed);
                return result;
            }

            var secret = MfaSecret.Generate();
            var qrCodeUri = secret.BuildQrCodeUri(user.Email ?? user.Id.Value.ToString());

            var setupChallenge = await _mfaChallengeFactory.SetupChallengeAsync(user, secret.Base32, qrCodeUri, ChallengeLifetime, cancellationToken);

            var setupResult = new MfaSetupResult(secret.Base32, qrCodeUri);
            var success = Result.Success(setupResult);
            return success;
        }

        public async Task<Result> VerifyAsync(int userId, string code, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                var result = Result.Failure(UserErrors.MfaCodeInvalid);
                return result;
            }

            var user = await _userRepository.GetById(new UserId(userId), cancellationToken);
            if (user is null)
            {
                var result = Result.Failure(UserErrors.NotFound(userId));
                return result;
            }
            if (!user.IsMfaEligible())
            {
                var result = Result.Failure(UserErrors.MfaNotAllowed);
                return result;
            }

            var challenge = await _mfaChallengeRepository.GetActiveByUser(userId, cancellationToken);
            if (challenge is null)
            {
                var result = Result.Failure(UserErrors.MfaChallengeNotFound);
                return result;
            }

            try
            {
                var secret = _mfaChallengeFactory.DecryptSecret(challenge);
                challenge.Confirm(secret, code.Trim(), DateTime.UtcNow);
                await _mfaChallengeRepository.Update(challenge, cancellationToken);

                user.EnableMfa();
                await _userRepository.Update(user, cancellationToken);
            }
            catch (DomainException ex)
            {
                _logger.LogInformation("Falha ao verificar MFA: {Message}", ex.Message);
                var result = Result.Failure(UserErrors.MfaCodeInvalid);
                return result;
            }

            var success = Result.Success();
            return success;
        }
    }
}
