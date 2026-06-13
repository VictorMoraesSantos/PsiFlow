using Auth.Application.Contracts;
using Auth.Application.DTOs.Users;
using Auth.Application.Mapping;
using Auth.Domain.Errors;
using Auth.Domain.Events;
using Auth.Domain.Filters;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using BuildingBlocks.Results;
using Core.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Auth.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository repository, ILogger<UserService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<int>> CountAsync(Expression<Func<UserDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = await _repository.GetAll(cancellationToken);
                var dtos = entities.Where(x => x is not null).Select(x => x!.ToDTO()).AsQueryable();
                var count = predicate is null ? dtos.Count() : dtos.Count(predicate);
                return Result.Success(count);
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
                if (entity is null) return Result.Failure<bool>(UserErrors.NotFound(id));

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
                if (user is null) return Result.Failure(UserErrors.NotFound(userId));

                user.Deactivate();
                user.AddDomainEvent(new UserDeactivatedDomainEvent(user.Id, user.TenantId, "self_delete"));
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
                return Result.Success<IEnumerable<UserDTO?>>(filtered.Cast<UserDTO?>());
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
                if (user is null) return Result.Failure<UserDTO>(UserErrors.NotFound(0));

                return Result.Success(user.ToDTO());
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
                return Result.Success<IEnumerable<UserDTO?>>(entities.Select(x => x is null ? null : (UserDTO?)x.ToDTO()));
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
                return Result.Success(((IEnumerable<UserDTO>)dtos, pagination));
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
                return entity is null
                    ? Result.Failure<UserDTO?>(UserErrors.NotFound(id))
                    : Result.Success<UserDTO?>(entity.ToDTO());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter usuario {Id}", id);
                return Result.Failure<UserDTO?>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateUserDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _repository.GetById(new UserId(dto.Id), cancellationToken);
                if (user is null) return Result.Failure<bool>(UserErrors.NotFound(dto.Id));

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
                if (user is null) return Result.Failure(UserErrors.NotFound(userId));

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
