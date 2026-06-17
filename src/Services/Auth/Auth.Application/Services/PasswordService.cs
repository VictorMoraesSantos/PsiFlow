using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using Auth.Domain.Entities;
using Auth.Domain.Errors;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using BuildingBlocks.Results;
using Core.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Auth.Application.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly ITokenRevocationService _tokenRevocationService;
        private readonly ILogger<PasswordService> _logger;

        public PasswordService(
            IUserRepository userRepository,
            UserManager<User> userManager,
            ITokenRevocationService tokenRevocationService,
            ILogger<PasswordService> logger)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _tokenRevocationService = tokenRevocationService;
            _logger = logger;
        }

        public async Task<Result> ChangeAsync(int userId, ChangePasswordDTO dto, CancellationToken cancellationToken = default)
        {
            if (!TryBuildPasswordPair(dto.NewPassword, dto.ConfirmNewPassword, out var newPassword, out var buildError) || buildError is not null)
            {
                var result = Result.Failure(buildError!);
                return result;
            }

            var user = await _userRepository.GetById(new UserId(userId), cancellationToken);
            if (user is null)
            {
                var result = Result.Failure(UserErrors.NotFound(userId));
                return result;
            }

            var changeResult = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, newPassword!.Value);
            if (!changeResult.Succeeded)
            {
                _logger.LogWarning("Falha ao alterar senha do usuario {UserId}: {Errors}", userId, string.Join("; ", changeResult.Errors.Select(e => e.Description)));
                var result = Result.Failure(UserErrors.InvalidCredentials);
                return result;
            }

            var success = Result.Success();
            return success;
        }

        public async Task<Result> ForgotAsync(ForgotPasswordDTO dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                var result = Result.Failure(ContactErrors.EmailRequired);
                return result;
            }

            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.FindByEmail(email, cancellationToken);
            if (user is null)
            {
                _logger.LogInformation("ForgotPassword para e-mail inexistente: {Email}", email);
                var result = Result.Success();
                return result;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            _logger.LogInformation("Reset de senha solicitado para {Email}", email);
            user.RequestPasswordReset(token);
            await _userRepository.Update(user, cancellationToken);

            var success = Result.Success();
            return success;
        }

        public async Task<Result> ResetAsync(ResetPasswordDTO dto, CancellationToken cancellationToken = default)
        {
            if (!TryBuildPasswordPair(dto.NewPassword, dto.ConfirmPassword, out var newPassword, out var buildError) || buildError is not null)
            {
                var result = Result.Failure(buildError!);
                return result;
            }

            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.FindByEmail(email, cancellationToken);
            if (user is null)
            {
                var result = Result.Failure(UserErrors.PasswordResetInvalid);
                return result;
            }

            var resetResult = await _userManager.ResetPasswordAsync(user, dto.Token, newPassword!.Value);
            if (!resetResult.Succeeded)
            {
                _logger.LogWarning("Falha no reset de senha para {Email}: {Errors}", email, string.Join("; ", resetResult.Errors.Select(e => e.Description)));
                var result = Result.Failure(UserErrors.PasswordResetInvalid);
                return result;
            }

            var revokeResult = await _tokenRevocationService.RevokeAllForUserAsync(user.Id.Value, cancellationToken);

            var success = Result.Success();
            return success;
        }

        private static bool TryBuildPasswordPair(string candidate, string confirmation, out PasswordPolicy? policy, out Error? error)
        {
            try
            {
                var newPassword = PasswordPolicy.Create(candidate);
                var confirm = PasswordPolicy.Create(confirmation);
                PasswordPolicy.EnsureMatch(newPassword, confirm);
                policy = newPassword;
                error = null;
                return true;
            }
            catch (DomainException ex)
            {
                policy = null;
                error = ex.Message.Contains("nao conferem", StringComparison.OrdinalIgnoreCase)
                    ? UserErrors.PasswordsDoNotMatch
                    : UserErrors.PasswordTooWeak;
                return false;
            }
        }
    }
}
