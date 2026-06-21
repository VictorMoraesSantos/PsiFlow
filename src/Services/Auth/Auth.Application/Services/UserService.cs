using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using Auth.Application.DTOs.Users;
using Auth.Application.Mapping;
using Auth.Application.Settings;
using Auth.Domain.Entities;
using Auth.Domain.Errors;
using Auth.Domain.Filters;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using BuildingBlocks.Results;
using Core.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace Auth.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly UserManager<User> _userManager;
        private readonly IPermissionAssignmentService _permissionAssignmentService;
        private readonly IOutboxService _outboxService;
        private readonly IConsentService _consentService;
        private readonly ILogger<UserService> _logger;
        private readonly AuthOptions _authOptions;

        public UserService(
            IUserRepository repository,
            UserManager<User> userManager,
            IPermissionAssignmentService permissionAssignmentService,
            IOutboxService outboxService,
            IConsentService consentService,
            ILogger<UserService> logger,
            IOptions<AuthOptions> authOptions)
        {
            _repository = repository;
            _userManager = userManager;
            _permissionAssignmentService = permissionAssignmentService;
            _outboxService = outboxService;
            _consentService = consentService;
            _logger = logger;
            _authOptions = authOptions.Value;
        }

        public async Task<Result<int>> CountAsync(Expression<Func<UserDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = await _repository.GetAll(cancellationToken);
                var dtos = entities.Where(x => x is not null).Select(x => x!.ToDTO()).AsQueryable();
                var count = predicate is null ? dtos.Count() : dtos.Count(predicate);
                var success = Result.Success(count);
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar usuarios");
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _repository.GetById(new UserId(id), cancellationToken);
                if (entity is null)
                    return Result.Failure<bool>(UserErrors.NotFound(id));

                entity.MarkAsDeleted();
                await _repository.Update(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir usuario {Id}", id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result> DeleteCurrentUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var id = new UserId(userId);
                var user = await _repository.GetById(id, cancellationToken);
                if (user is null)
                    return Result.Failure(UserErrors.NotFound(userId));

                user.Deactivate("self_delete");
                await _repository.Update(user, cancellationToken);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir usuario corrente {UserId}", userId);
                return Result.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                foreach (var id in ids)
                {
                    var entity = await _repository.GetById(new UserId(id), cancellationToken);
                    if (entity is null) continue;
                    entity.MarkAsDeleted();
                    await _repository.Update(entity, cancellationToken);
                }
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir usuarios em lote");
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<UserDTO?>>> FindAsync(Expression<Func<UserDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = await _repository.GetAll(cancellationToken);
                var dtos = entities.Where(x => x is not null).Select(x => x!.ToDTO()).AsQueryable();
                var filtered = dtos.Where(predicate).ToList();
                var success = Result.Success<IEnumerable<UserDTO?>>(filtered.Cast<UserDTO?>());
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuarios por predicado");
                return Result.Failure<IEnumerable<UserDTO?>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<UserDTO>> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return Result.Failure<UserDTO>(ContactErrors.EmailRequired);

                var user = await _repository.FindByEmail(email, cancellationToken);
                if (user is null)
                    return Result.Failure<UserDTO>(UserErrors.NotFound(0));

                var dto = user.ToDTO();
                return Result.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuario por e-mail {Email}", email);
                return Result.Failure<UserDTO>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<UserDTO?>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = await _repository.GetAll(cancellationToken);
                var list = entities.Select(x => x is null ? null : (UserDTO?)x.ToDTO());
                return Result.Success<IEnumerable<UserDTO?>>(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar usuarios");
                return Result.Failure<IEnumerable<UserDTO?>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<UserDTO> Items, PaginationData Pagination)>> GetByFilterAsync(UserFilter filter, CancellationToken cancellationToken)
        {
            try
            {
                var (items, total) = await _repository.FindByFilter(filter, cancellationToken);
                var dtos = items.Select(x => x.ToDTO()).ToList();
                var pageSize = filter.PageSize ?? 50;
                var totalPages = pageSize > 0 ? (int)Math.Ceiling(total / (double)pageSize) : 0;
                var pagination = new PaginationData(filter.Page, pageSize, total, totalPages);
                var payload = ((IEnumerable<UserDTO>)dtos, pagination);
                return Result.Success(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar usuarios por filtro");
                return Result.Failure<(IEnumerable<UserDTO> Items, PaginationData Pagination)>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<UserDTO?>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _repository.GetById(new UserId(id), cancellationToken);
                if (entity is null)
                    return Result.Failure<UserDTO?>(UserErrors.NotFound(id));

                var dto = entity.ToDTO();
                return Result.Success<UserDTO?>(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter usuario {Id}", id);
                return Result.Failure<UserDTO?>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<MeResponse>> GetMeAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _repository.GetById(new UserId(userId), cancellationToken);
            if (user is null)
                return Result.Failure<MeResponse>(UserErrors.NotFound(userId));

            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);
            var permissions = claims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .OrderBy(v => v, StringComparer.Ordinal)
                .ToArray();

            var response = new MeResponse(
                user.Id.Value,
                user.TenantId.Value,
                user.Email ?? string.Empty,
                user.Role,
                user.Name.FullName,
                user.IsActive,
                user.EmailConfirmed,
                user.IsMfaEnabled,
                permissions,
                roles.ToArray());

            return Result.Success(response);
        }

        public async Task<Result<RegisterResult>> RegisterAsync(RegisterDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto is null)
                return Result.Failure<RegisterResult>(UserErrors.CreateError);

            var email = dto.Email.Trim().ToLowerInvariant();
            var existing = await _repository.FindByEmail(email, cancellationToken);
            if (existing is not null)
                return Result.Failure<RegisterResult>(UserErrors.RegistrationUnavailable);

            var nameResult = TryBuildName(dto);
            if (!nameResult.IsSuccess)
                return Result.Failure<RegisterResult>(nameResult.Error!);

            var contact = new Contact(dto.Email, dto.Phone);
            var termsVersion = DocumentVersion.Create(dto.AcceptedTermsVersion, nameof(dto.AcceptedTermsVersion));
            var privacyVersion = DocumentVersion.Create(dto.AcceptedPrivacyVersion, nameof(dto.AcceptedPrivacyVersion));
            if (termsVersion is null || privacyVersion is null)
                return Result.Failure<RegisterResult>(UserErrors.TermsNotAccepted);

            var user = User.Register(
                nameResult.Value!,
                contact,
                dto.Role,
                tenantId: null,
                dto.Role == UserRole.Psychologist ? dto.Crp : null);

            var identity = await _userManager.CreateAsync(user, dto.Password);
            if (!identity.Succeeded)
            {
                _logger.LogWarning("Falha ao registrar usuario {Email}: {Errors}", email, string.Join("; ", identity.Errors.Select(e => e.Description)));
                return Result.Failure<RegisterResult>(UserErrors.RegistrationUnavailable);
            }

            if (dto.Role == UserRole.Psychologist && user.TenantId.Value == 0)
            {
                user.AttachTenant(new TenantId(user.Id));
                var tenantUpdate = await _userManager.UpdateAsync(user);
                if (!tenantUpdate.Succeeded)
                    _logger.LogWarning("Falha ao atribuir tenant do psychologist {UserId}: {Errors}", user.Id, string.Join("; ", tenantUpdate.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(user, user.Role);
            await _permissionAssignmentService.AssignDefaultAsync(user, cancellationToken);

            if (_authOptions.AutoConfirmEmails)
            {
                user.ConfirmEmail();
                var emailUpdate = await _userManager.UpdateAsync(user);
                if (!emailUpdate.Succeeded)
                    _logger.LogWarning("Falha ao auto-confirmar e-mail do usuario {UserId}: {Errors}", user.Id, string.Join("; ", emailUpdate.Errors.Select(e => e.Description)));
            }

            var correlationId = Guid.NewGuid();
            user.RegisterUser(correlationId);

            var consentDto = new ConsentDTO("terms_privacy", dto.AcceptedTermsVersion, dto.AcceptedPrivacyVersion, null, null);
            var consentResult = await _consentService.RecordAsync(user.Id.Value, consentDto, cancellationToken);
            if (!consentResult.IsSuccess)
            {
                _logger.LogWarning("Falha ao registrar consentimento inicial do usuario {UserId}: {Error}", user.Id, consentResult.Error?.Description);
            }

            await _outboxService.PersistEventsAsync(user.Id.Value, nameof(User), user.DomainEvents, correlationId, cancellationToken);
            user.ClearDomainEvents();

            var registered = new RegisterResult(user.Id.Value, user.TenantId.Value, user.Email ?? string.Empty, user.Role);
            return Result.Success(registered);
        }

        private static Result<Name> TryBuildName(RegisterDTO dto)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(dto.FirstName) && !string.IsNullOrWhiteSpace(dto.LastName))
                    return Result.Success(new Name(dto.FirstName, dto.LastName););

                if (!string.IsNullOrWhiteSpace(dto.FirstName))
                    return Result.Success(new Name(dto.FirstName));

                if (!string.IsNullOrWhiteSpace(dto.FullName))
                    return Result.Success(new Name(dto.FullName));

                return Result.Failure<Name>(NameErrors.NullName);
            }
            catch (Exception)
            {
                return Result.Failure<Name>(NameErrors.NullName);
            }
        }

        public async Task<Result> BeginLoginAsync(User user, CancellationToken cancellationToken = default)
        {
            user.BeginLogin();
            await _repository.Update(user, cancellationToken);

            return Result.Success();
        }

        public async Task<Result> AttachTenantAsync(User user, UserId tenantId, CancellationToken cancellationToken = default)
        {
            user.AttachTenant(new TenantId(tenantId));
            await _repository.Update(user, cancellationToken);

            return Result.Success();
        }

        public async Task<Result<bool>> UpdateAsync(UpdateUserDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _repository.GetById(new UserId(dto.Id), cancellationToken);
                if (user is null)
                    return Result.Failure<bool>(UserErrors.NotFound(dto.Id));

                var name = new Name(dto.FullName);
                var contact = new Contact(dto.Email, dto.Phone);
                user.UpdateProfile(name, contact);
                await _repository.Update(user, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar usuario {Id}", dto.Id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result> UpdateCurrentUserProfileAsync(int userId, UpdateCurrentUserProfileDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var id = new UserId(userId);
                var user = await _repository.GetById(id, cancellationToken);
                if (user is null)
                    return Result.Failure(UserErrors.NotFound(userId));

                var name = new Name(dto.FullName);
                var contact = new Contact(dto.Email, dto.Phone);
                user.UpdateProfile(name, contact);
                await _repository.Update(user, cancellationToken);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar perfil do usuario corrente {UserId}", userId);
                return Result.Failure(Error.Failure(ex.Message));
            }
        }
    }
}
