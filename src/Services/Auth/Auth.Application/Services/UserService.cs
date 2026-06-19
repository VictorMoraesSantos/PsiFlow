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
        private readonly IUserOutboxService _userOutboxService;
        private readonly ILogger<UserService> _logger;
        private readonly AuthOptions _authOptions;

        public UserService(
            IUserRepository repository,
            UserManager<User> userManager,
            IPermissionAssignmentService permissionAssignmentService,
            IUserOutboxService userOutboxService,
            ILogger<UserService> logger,
            IOptions<AuthOptions> authOptions)
        {
            _repository = repository;
            _userManager = userManager;
            _permissionAssignmentService = permissionAssignmentService;
            _userOutboxService = userOutboxService;
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
                var failure = Result.Failure<int>(Error.Failure(ex.Message));
                return failure;
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _repository.GetById(new UserId(id), cancellationToken);
                if (entity is null)
                {
                    var failure = Result.Failure<bool>(UserErrors.NotFound(id));
                    return failure;
                }

                entity.MarkAsDeleted();
                await _repository.Update(entity, cancellationToken);
                var success = Result.Success(true);
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir usuario {Id}", id);
                var failure = Result.Failure<bool>(Error.Failure(ex.Message));
                return failure;
            }
        }

        public async Task<Result> DeleteCurrentUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var id = new UserId(userId);
                var user = await _repository.GetById(id, cancellationToken);
                if (user is null)
                {
                    var failure = Result.Failure(UserErrors.NotFound(userId));
                    return failure;
                }

                user.Deactivate("self_delete");
                await _repository.Update(user, cancellationToken);
                var success = Result.Success();
                return success;
            }
            catch (DomainException ex)
            {
                var failure = Result.Failure(Error.Failure(ex.Message));
                return failure;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir usuario corrente {UserId}", userId);
                var failure = Result.Failure(Error.Failure(ex.Message));
                return failure;
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
                var success = Result.Success(true);
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir usuarios em lote");
                var failure = Result.Failure<bool>(Error.Failure(ex.Message));
                return failure;
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
                var failure = Result.Failure<IEnumerable<UserDTO?>>(Error.Failure(ex.Message));
                return failure;
            }
        }

        public async Task<Result<UserDTO>> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    var failure = Result.Failure<UserDTO>(ContactErrors.EmailRequired);
                    return failure;
                }

                var user = await _repository.FindByEmail(email, cancellationToken);
                if (user is null)
                {
                    var failure = Result.Failure<UserDTO>(UserErrors.NotFound(0));
                    return failure;
                }

                var dto = user.ToDTO();
                var success = Result.Success(dto);
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuario por e-mail {Email}", email);
                var failure = Result.Failure<UserDTO>(Error.Failure(ex.Message));
                return failure;
            }
        }

        public async Task<Result<IEnumerable<UserDTO?>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = await _repository.GetAll(cancellationToken);
                var list = entities.Select(x => x is null ? null : (UserDTO?)x.ToDTO());
                var success = Result.Success<IEnumerable<UserDTO?>>(list);
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar usuarios");
                var failure = Result.Failure<IEnumerable<UserDTO?>>(Error.Failure(ex.Message));
                return failure;
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
                var success = Result.Success(payload);
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar usuarios por filtro");
                var failure = Result.Failure<(IEnumerable<UserDTO> Items, PaginationData Pagination)>(Error.Failure(ex.Message));
                return failure;
            }
        }

        public async Task<Result<UserDTO?>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _repository.GetById(new UserId(id), cancellationToken);
                if (entity is null)
                {
                    var failure = Result.Failure<UserDTO?>(UserErrors.NotFound(id));
                    return failure;
                }
                else
                {
                    var dto = entity.ToDTO();
                    var success = Result.Success<UserDTO?>(dto);
                    return success;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter usuario {Id}", id);
                var failure = Result.Failure<UserDTO?>(Error.Failure(ex.Message));
                return failure;
            }
        }

        public async Task<Result<MeResponse>> GetMeAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _repository.GetById(new UserId(userId), cancellationToken);
            if (user is null)
            {
                var result = Result.Failure<MeResponse>(UserErrors.NotFound(userId));
                return result;
            }

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
            var success = Result.Success(response);
            return success;
        }

        public async Task<Result<RegisterResult>> RegisterAsync(RegisterDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto is null)
            {
                var result = Result.Failure<RegisterResult>(UserErrors.CreateError);
                return result;
            }

            var email = dto.Email.Trim().ToLowerInvariant();
            var existing = await _repository.FindByEmail(email, cancellationToken);
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
                dto.Role == UserRole.Psychologist ? dto.Crp : null,
                termsVersion,
                privacyVersion);

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
            user.RecordConsent(termsVersion, privacyVersion);
            user.RegisterUser(correlationId);

            await _userOutboxService.PersistEventsAsync(user, correlationId, cancellationToken);

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

        public async Task<Result> BeginLoginAsync(User user, CancellationToken cancellationToken = default)
        {
            user.BeginLogin();
            await _repository.Update(user, cancellationToken);

            var success = Result.Success();
            return success;
        }

        public async Task<Result> AttachTenantAsync(User user, UserId tenantId, CancellationToken cancellationToken = default)
        {
            user.AttachTenant(new TenantId(tenantId));
            await _repository.Update(user, cancellationToken);

            var success = Result.Success();
            return success;
        }

        public async Task<Result<bool>> UpdateAsync(UpdateUserDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _repository.GetById(new UserId(dto.Id), cancellationToken);
                if (user is null)
                {
                    var failure = Result.Failure<bool>(UserErrors.NotFound(dto.Id));
                    return failure;
                }

                var name = new Name(dto.FullName);
                var contact = new Contact(dto.Email, dto.Phone);
                user.UpdateProfile(name, contact);
                await _repository.Update(user, cancellationToken);
                var success = Result.Success(true);
                return success;
            }
            catch (DomainException ex)
            {
                var failure = Result.Failure<bool>(Error.Failure(ex.Message));
                return failure;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar usuario {Id}", dto.Id);
                var failure = Result.Failure<bool>(Error.Failure(ex.Message));
                return failure;
            }
        }

        public async Task<Result> UpdateCurrentUserProfileAsync(int userId, UpdateCurrentUserProfileDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var id = new UserId(userId);
                var user = await _repository.GetById(id, cancellationToken);
                if (user is null)
                {
                    var failure = Result.Failure(UserErrors.NotFound(userId));
                    return failure;
                }

                var name = new Name(dto.FullName);
                var contact = new Contact(dto.Email, dto.Phone);
                user.UpdateProfile(name, contact);
                await _repository.Update(user, cancellationToken);
                var success = Result.Success();
                return success;
            }
            catch (DomainException ex)
            {
                var failure = Result.Failure(Error.Failure(ex.Message));
                return failure;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar perfil do usuario corrente {UserId}", userId);
                var failure = Result.Failure(Error.Failure(ex.Message));
                return failure;
            }
        }
    }
}
