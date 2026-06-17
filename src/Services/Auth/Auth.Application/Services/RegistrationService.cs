using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using Auth.Application.Settings;
using Auth.Domain.Entities;
using Auth.Domain.Errors;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using BuildingBlocks.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Auth.Application.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IPermissionAssignmentService _permissionAssignmentService;
        private readonly IUserOutboxService _userOutboxService;
        private readonly ILogger<RegistrationService> _logger;
        private readonly AuthOptions _authOptions;

        public RegistrationService(
            IUserRepository userRepository,
            UserManager<User> userManager,
            IPermissionAssignmentService permissionAssignmentService,
            IUserOutboxService userOutboxService,
            ILogger<RegistrationService> logger,
            IOptions<AuthOptions> authOptions)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _permissionAssignmentService = permissionAssignmentService;
            _userOutboxService = userOutboxService;
            _logger = logger;
            _authOptions = authOptions.Value;
        }

        public async Task<Result<RegisterResult>> RegisterAsync(RegisterDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto is null)
            {
                var result = Result.Failure<RegisterResult>(UserErrors.CreateError);
                return result;
            }

            var email = dto.Email.Trim().ToLowerInvariant();
            var existing = await _userRepository.FindByEmail(email, cancellationToken);
            if (existing is not null)
            {
                var result = Result.Failure<RegisterResult>(UserErrors.RegistrationUnavailable);
                return result;
            }

            var nameResult = TryBuildName(dto);
            if (!nameResult.IsSuccess)
            {
                var result = Result.Failure<RegisterResult>(nameResult.Error!);
                return result;
            }

            var contact = new Contact(dto.Email, dto.Phone);

            var termsVersion = DocumentVersion.Create(dto.AcceptedTermsVersion, nameof(dto.AcceptedTermsVersion));
            var privacyVersion = DocumentVersion.Create(dto.AcceptedPrivacyVersion, nameof(dto.AcceptedPrivacyVersion));
            if (termsVersion is null || privacyVersion is null)
            {
                var result = Result.Failure<RegisterResult>(UserErrors.TermsNotAccepted);
                return result;
            }

            var user = User.Register(
                nameResult.Value!,
                contact,
                dto.Role,
                tenantId: null,
                crp: dto.Role == UserRole.Psychologist ? dto.Crp : null,
                termsVersion: termsVersion,
                privacyVersion: privacyVersion);

            var identity = await _userManager.CreateAsync(user, dto.Password);
            if (!identity.Succeeded)
            {
                _logger.LogWarning("Falha ao registrar usuario {Email}: {Errors}", email, string.Join("; ", identity.Errors.Select(e => e.Description)));
                var result = Result.Failure<RegisterResult>(UserErrors.RegistrationUnavailable);
                return result;
            }

            if (dto.Role == UserRole.Psychologist && user.TenantId.Value == 0)
            {
                user.AttachTenant(new TenantId(user.Id));
                var tenantUpdate = await _userManager.UpdateAsync(user);
                if (!tenantUpdate.Succeeded)
                    _logger.LogWarning("Falha ao atribuir tenant do psychologist {UserId}: {Errors}", user.Id, string.Join("; ", tenantUpdate.Errors.Select(e => e.Description)));
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, user.Role);
            var permissionResult = await _permissionAssignmentService.AssignDefaultAsync(user, cancellationToken);

            if (_authOptions.AutoConfirmEmails)
            {
                user.ConfirmEmail();
                var emailUpdate = await _userManager.UpdateAsync(user);
                if (!emailUpdate.Succeeded)
                    _logger.LogWarning("Falha ao auto-confirmar e-mail do usuario {UserId}: {Errors}", user.Id, string.Join("; ", emailUpdate.Errors.Select(e => e.Description)));
            }

            var correlationId = Guid.NewGuid();
            user.RecordConsent(termsVersion, privacyVersion);
            user.RegisterUser(correlationId);

            var outboxResult = await _userOutboxService.PersistEventsAsync(user, correlationId, cancellationToken);

            var registered = new RegisterResult(user.Id.Value, user.TenantId.Value, user.Email ?? string.Empty, user.Role);
            var success = Result.Success(registered);
            return success;
        }

        private static Result<Name> TryBuildName(RegisterDTO dto)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(dto.FirstName) && !string.IsNullOrWhiteSpace(dto.LastName))
                {
                    var name = new Name(dto.FirstName, dto.LastName);
                    var success = Result.Success(name);
                    return success;
                }
                if (!string.IsNullOrWhiteSpace(dto.FirstName))
                {
                    var name = new Name(dto.FirstName);
                    var success = Result.Success(name);
                    return success;
                }
                if (!string.IsNullOrWhiteSpace(dto.FullName))
                {
                    var name = new Name(dto.FullName);
                    var success = Result.Success(name);
                    return success;
                }
                var failure = Result.Failure<Name>(NameErrors.NullName);
                return failure;
            }
            catch (Exception)
            {
                var failure = Result.Failure<Name>(NameErrors.NullName);
                return failure;
            }
        }
    }
}
