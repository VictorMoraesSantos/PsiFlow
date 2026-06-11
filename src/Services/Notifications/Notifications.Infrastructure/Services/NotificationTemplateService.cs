using BuildingBlocks.Results;
using Microsoft.Extensions.Logging;
using Notifications.Application.Contracts;
using Notifications.Application.DTOs.NotificationTemplate;
using Notifications.Application.Mapping;
using Notifications.Domain.Errors;
using Notifications.Domain.Filters;
using Notifications.Domain.Repositories;
using System.Linq.Expressions;

namespace Notifications.Infrastructure.Services
{
    public sealed class NotificationTemplateService : INotificationTemplateService
    {
        private readonly INotificationTemplateRepository _repository;
        private readonly ILogger<NotificationTemplateService> _logger;
        public NotificationTemplateService(INotificationTemplateRepository repository, ILogger<NotificationTemplateService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<int>> CreateAsync(CreateNotificationTemplateDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Key))
                    return Result.Failure<int>(NotificationTemplateErrors.CreateError);

                var entity = dto.ToEntity();
                await _repository.Create(entity, cancellationToken);
                return Result.Success(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar NotificationTemplate");
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateNotificationTemplateDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = dtos.Select(d => d.ToEntity()).ToList();
                await _repository.CreateRange(entities, cancellationToken);
                return Result.Success<IEnumerable<int>>(entities.Select(e => 0));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar notification templates em lote");
                return Result.Failure<IEnumerable<int>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateNotificationTemplateDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _repository.GetById(dto.Id, cancellationToken);
                if (entity is null) return Result.Failure<bool>(NotificationTemplateErrors.NotFound(dto.Id));
                entity.TenantId = dto.TenantId;
                entity.Key = dto.Key;
                entity.Channel = dto.Channel;
                entity.Name = dto.Name;
                entity.Status = dto.Status;
                entity.IsActive = dto.IsActive;
                entity.MarkAsUpdated();
                await _repository.Update(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar NotificationTemplate {Id}", dto.Id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _repository.GetById(id, cancellationToken);
                if (entity is null) return Result.Failure<bool>(NotificationTemplateErrors.NotFound(id));
                await _repository.Delete(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir NotificationTemplate {Id}", id);
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
                _logger.LogError(ex, "Erro ao excluir notification templates em lote");
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<NotificationTemplateDTO?>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetById(id, cancellationToken);
            return entity is null ? Result.Failure<NotificationTemplateDTO?>(NotificationTemplateErrors.NotFound(id)) : Result.Success<NotificationTemplateDTO?>(entity.ToDTO());
        }

        public async Task<Result<IEnumerable<NotificationTemplateDTO?>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAll(cancellationToken);
            return Result.Success(entities.Select(x => x is null ? null : (NotificationTemplateDTO?)x.ToDTO()));
        }

        public async Task<Result<(IEnumerable<NotificationTemplateDTO?> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var filter = new NotificationTemplateFilterDTO(null, null, page, pageSize);
            var result = await GetByFilterAsync(filter, cancellationToken);
            if (!result.IsSuccess) return Result.Failure<(IEnumerable<NotificationTemplateDTO?> Items, int TotalCount)>(result.Error!);
            return Result.Success((result.Value!.Items.Cast<NotificationTemplateDTO?>(), result.Value.Pagination.TotalItems ?? 0));
        }

        public async Task<Result<IEnumerable<NotificationTemplateDTO?>>> FindAsync(Expression<Func<NotificationTemplateDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAll(cancellationToken);
            var dtos = entities.Where(x => x is not null).Select(x => x!.ToDTO()).AsQueryable();
            var filtered = dtos.Where(predicate).ToList();
            return Result.Success(filtered.Cast<NotificationTemplateDTO?>());
        }

        public async Task<Result<int>> CountAsync(Expression<Func<NotificationTemplateDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAll(cancellationToken);
            var dtos = entities.Where(x => x is not null).Select(x => x!.ToDTO()).AsQueryable();
            var count = predicate is null ? dtos.Count() : dtos.Count(predicate);
            return Result.Success(count);
        }

        public async Task<Result<(IEnumerable<NotificationTemplateDTO> Items, PaginationData Pagination)>> GetByFilterAsync(NotificationTemplateFilterDTO filter, CancellationToken cancellationToken = default)
        {
            var queryFilter = new NotificationTemplateQueryFilter(filter.TenantId, filter.Search);
            var (items, total) = await _repository.FindByFilter(queryFilter, cancellationToken);
            var dtos = items.Select(x => x.ToDTO()).ToList();
            var pagination = new PaginationData(filter.Page, filter.PageSize, total, (int)Math.Ceiling(total / (double)(filter.PageSize ?? 20)));
            return Result.Success(((IEnumerable<NotificationTemplateDTO>)dtos, pagination));
        }
    }
}
