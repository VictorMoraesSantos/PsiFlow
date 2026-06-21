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
                return Result.Failure<string>(ContactErrors.EmailRequired);

            var user = await _userRepository.FindByEmail(email.Trim().ToLowerInvariant(), cancellationToken);
            if (user is null)
                return Result.Failure<string>(ContactErrors.EmailNotFound);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            user.RequestEmailVerification(token);
            await _userRepository.Update(user, cancellationToken);

            return Result.Success(token);
        }

        public async Task<Result> VerifyAsync(string email, string token, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
                return Result.Failure(UserErrors.InvalidCredentials);

            var user = await _userRepository.FindByEmail(email.Trim().ToLowerInvariant(), cancellationToken);
            if (user is null)
                return Result.Failure(UserErrors.NotFound(0));

            var confirmResult = await _userManager.ConfirmEmailAsync(user, token);
            if (!confirmResult.Succeeded)
            {
                _logger.LogWarning("Falha ao verificar e-mail: {Errors}", string.Join("; ", confirmResult.Errors.Select(e => e.Description)));
                return Result.Failure(UserErrors.InvalidCredentials);
            }
            user.ConfirmEmail();
            await _userRepository.Update(user, cancellationToken);

            return Result.Success();
        }
    }
}
