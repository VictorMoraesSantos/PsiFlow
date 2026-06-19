using Auth.Application.Contracts;
using Auth.Domain.Entities;
using Auth.Domain.Errors;
using Auth.Domain.Repositories;
using BuildingBlocks.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Auth.Application.Services
{
    public class EmailVerificationService : IEmailVerificationService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<EmailVerificationService> _logger;

        public EmailVerificationService(
            IUserRepository userRepository,
            UserManager<User> userManager,
            ILogger<EmailVerificationService> logger)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<Result<string>> RequestAsync(string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                var failure = Result.Failure<string>(ContactErrors.EmailRequired);
                return failure;
            }
            var user = await _userRepository.FindByEmail(email.Trim().ToLowerInvariant(), cancellationToken);
            if (user is null)
            {
                var success = Result.Success<string>(string.Empty);
                return success;
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            user.RequestEmailVerification(token);
            await _userRepository.Update(user, cancellationToken);

            var successResult = Result.Success(token);
            return successResult;
        }

        public async Task<Result> VerifyAsync(string email, string token, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                var failure = Result.Failure(UserErrors.InvalidCredentials);
                return failure;
            }
            var user = await _userRepository.FindByEmail(email.Trim().ToLowerInvariant(), cancellationToken);
            if (user is null)
            {
                var failure = Result.Failure(UserErrors.NotFound(0));
                return failure;
            }

            var confirmResult = await _userManager.ConfirmEmailAsync(user, token);
            if (!confirmResult.Succeeded)
            {
                _logger.LogWarning("Falha ao verificar e-mail: {Errors}", string.Join("; ", confirmResult.Errors.Select(e => e.Description)));
                var failure = Result.Failure(UserErrors.InvalidCredentials);
                return failure;
            }
            user.ConfirmEmail();
            await _userRepository.Update(user, cancellationToken);

            var success = Result.Success();
            return success;
        }
    }
}
