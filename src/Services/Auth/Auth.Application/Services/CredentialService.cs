using Auth.Application.Contracts;
using Auth.Domain.Entities;
using Auth.Domain.Errors;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using BuildingBlocks.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Auth.Application.Services
{
    public class CredentialService : ICredentialService
    {
        private readonly IUserRepository _userRepository;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<CredentialService> _logger;

        public CredentialService(
            IUserRepository userRepository,
            SignInManager<User> signInManager,
            ILogger<CredentialService> logger)
        {
            _userRepository = userRepository;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<Result<AuthenticatedUser>> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return Result.Failure<AuthenticatedUser>(UserErrors.InvalidCredentials);

            var normalized = email.Trim().ToLowerInvariant();
            var user = await _userRepository.FindByEmail(normalized, cancellationToken);
            if (user is null)
            {
                _logger.LogInformation("Login falhou: usuario nao encontrado para {Email}", normalized);
                return Result.Failure<AuthenticatedUser>(UserErrors.InvalidCredentials);
            }

            if (!user.IsActive)
            {
                _logger.LogInformation("Login falhou: usuario inativo {UserId}", user.Id);
                return Result.Failure<AuthenticatedUser>(UserErrors.AlreadyInactive);
            }

            if (!user.EmailConfirmed)
            {
                _logger.LogInformation("Login falhou: e-mail nao confirmado {UserId}", user.Id);
                return Result.Failure<AuthenticatedUser>(UserErrors.EmailNotConfirmed);
            }

            var signIn = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
            if (signIn.IsLockedOut)
            {
                _logger.LogInformation("Login falhou: conta bloqueada {UserId} ate {LockoutEnd}", user.Id, user.LockoutEnd);
                return Result.Failure<AuthenticatedUser>(UserErrors.UserLockedOut);
            }
            if (!signIn.Succeeded)
            {
                _logger.LogInformation("Login falhou: senha invalida para {UserId}", user.Id);
                return Result.Failure<AuthenticatedUser>(UserErrors.InvalidCredentials);
            }

            return Result.Success(new AuthenticatedUser(user, user.IsMfaEnabled));
        }
    }
}
