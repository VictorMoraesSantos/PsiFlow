using BuildingBlocks.Results;
using Microsoft.Extensions.Logging;
using Sessions.Application.Contracts;
using Sessions.Application.DTOs.Session;
using Sessions.Application.Mapping;
using Sessions.Domain.Errors;
using Sessions.Domain.Filters;
using Sessions.Domain.Repositories;
using System.Linq.Expressions;

namespace Sessions.Application.Services
{
    public sealed class SessionService : ISessionService
    {
        private readonly ISessionRepository _repository;
        private readonly ILogger<SessionService> _logger;
        public SessionService(ISessionRepository repository, ILogger<SessionService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<int>> CreateAsync(CreateSessionDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result.Failure<int>(SessionErrors.CreateError);
                if (dto.EndsAt <= dto.StartsAt)
                    return Result.Failure<int>(SessionErrors.CreateError);

                var entity = dto.ToEntity();
                await _repository.Create(entity, cancellationToken);
                return Result.Success(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar Session");
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateSessionDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = dtos.Select(d => d.ToEntity()).ToList();
                await _repository.CreateRange(entities, cancellationToken);
                return Result.Success<IEnumerable<int>>(entities.Select(e => 0));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar sessions em lote");
                return Result.Failure<IEnumerable<int>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateSessionDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _repository.GetById(dto.Id, cancellationToken);
                if (entity is null) return Result.Failure<bool>(SessionErrors.NotFound(dto.Id));
                entity.TenantId = dto.TenantId;
                entity.Name = dto.Name;
                entity.AppointmentId = dto.AppointmentId;
                entity.PatientId = dto.PatientId;
                entity.PsychologistId = dto.PsychologistId;
                entity.StartsAt = dto.StartsAt;
                entity.EndsAt = dto.EndsAt;
                entity.Status = dto.Status;
                entity.Modality = dto.Modality;
                entity.MarkAsUpdated();
                await _repository.Update(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar Session {Id}", dto.Id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _repository.GetById(id, cancellationToken);
                if (entity is null) return Result.Failure<bool>(SessionErrors.NotFound(id));
                await _repository.Delete(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir Session {Id}", id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                foreach (var id in ids)
                {
                    var entity = await _repository.GetById(id, cancellationToken);
                    if (entity is not null) await _repository.Delete(entity, cancellationToken);
                }
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir sessions em lote");
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<SessionDTO?>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetById(id, cancellationToken);
            return entity is null ? Result.Failure<SessionDTO?>(SessionErrors.NotFound(id)) : Result.Success<SessionDTO?>(entity.ToDTO());
        }

        public async Task<Result<IEnumerable<SessionDTO?>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAll(cancellationToken);
            return Result.Success(entities.Select(x => x is null ? null : (SessionDTO?)x.ToDTO()));
        }

        public async Task<Result<(IEnumerable<SessionDTO?> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var filter = new SessionFilterDTO(null, null, page, pageSize);
            var result = await GetByFilterAsync(filter, cancellationToken);
            if (!result.IsSuccess) return Result.Failure<(IEnumerable<SessionDTO?> Items, int TotalCount)>(result.Error!);
            return Result.Success((result.Value!.Items.Cast<SessionDTO?>(), result.Value.Pagination.TotalItems ?? 0));
        }

        public async Task<Result<IEnumerable<SessionDTO?>>> FindAsync(Expression<Func<SessionDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAll(cancellationToken);
            var dtos = entities.Where(x => x is not null).Select(x => x!.ToDTO()).AsQueryable();
            var filtered = dtos.Where(predicate).ToList();
            return Result.Success(filtered.Cast<SessionDTO?>());
        }

        public async Task<Result<int>> CountAsync(Expression<Func<SessionDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAll(cancellationToken);
            var dtos = entities.Where(x => x is not null).Select(x => x!.ToDTO()).AsQueryable();
            var count = predicate is null ? dtos.Count() : dtos.Count(predicate);
            return Result.Success(count);
        }

        public async Task<Result<(IEnumerable<SessionDTO> Items, PaginationData Pagination)>> GetByFilterAsync(SessionFilterDTO filter, CancellationToken cancellationToken = default)
        {
            var queryFilter = new SessionQueryFilter(filter.TenantId, filter.Search);
            var (items, total) = await _repository.FindByFilter(queryFilter, cancellationToken);
            var dtos = items.Select(x => x.ToDTO()).ToList();
            var pagination = new PaginationData(filter.Page, filter.PageSize, total, (int)Math.Ceiling(total / (double)(filter.PageSize ?? 20)));
            return Result.Success(((IEnumerable<SessionDTO>)dtos, pagination));
        }
    }
}
