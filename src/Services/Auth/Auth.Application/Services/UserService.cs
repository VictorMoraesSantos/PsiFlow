using Auth.Application.Contracts;
using Auth.Application.DTOs.Users;
using Auth.Application.Mapping;
using Auth.Domain.Errors;
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
