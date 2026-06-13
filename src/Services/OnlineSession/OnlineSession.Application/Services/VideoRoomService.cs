using BuildingBlocks.Results;
using Microsoft.Extensions.Logging;
using OnlineSession.Application.Contracts;
using OnlineSession.Application.DTOs.VideoRoom;
using OnlineSession.Application.Mapping;
using OnlineSession.Domain.Errors;
using OnlineSession.Domain.Filters;
using OnlineSession.Domain.Repositories;
using System.Linq.Expressions;

namespace OnlineSession.Application.Services;

public sealed class VideoRoomService : IVideoRoomService
{
    private readonly IVideoRoomRepository _repository;
    private readonly ILogger<VideoRoomService> _logger;
    public VideoRoomService(IVideoRoomRepository repository, ILogger<VideoRoomService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<int>> CreateAsync(CreateVideoRoomDTO dto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result.Failure<int>(VideoRoomErrors.CreateError);

            var entity = dto.ToEntity();
            await _repository.Create(entity, cancellationToken);
            return Result.Success(0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar VideoRoom");
            return Result.Failure<int>(Error.Failure(ex.Message));
        }
    }

    public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateVideoRoomDTO> dtos, CancellationToken cancellationToken = default)
    {
        try
        {
            var entities = dtos.Select(d => d.ToEntity()).ToList();
            await _repository.CreateRange(entities, cancellationToken);
            return Result.Success<IEnumerable<int>>(entities.Select(e => 0));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar video rooms em lote");
            return Result.Failure<IEnumerable<int>>(Error.Failure(ex.Message));
        }
    }

    public async Task<Result<bool>> UpdateAsync(UpdateVideoRoomDTO dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _repository.GetById(dto.Id, cancellationToken);
            if (entity is null) return Result.Failure<bool>(VideoRoomErrors.NotFound(dto.Id));
            entity.TenantId = dto.TenantId;
            entity.SessionId = dto.SessionId;
            entity.Name = dto.Name;
            entity.Provider = dto.Provider;
            entity.UrlEncrypted = dto.UrlEncrypted;
            entity.UrlHash = dto.UrlHash;
            entity.Instructions = dto.Instructions;
            entity.CreatedBy = dto.CreatedBy;
            entity.Status = dto.Status;
            entity.MarkAsUpdated();
            await _repository.Update(entity, cancellationToken);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar VideoRoom {Id}", dto.Id);
            return Result.Failure<bool>(Error.Failure(ex.Message));
        }
    }

    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _repository.GetById(id, cancellationToken);
            if (entity is null) return Result.Failure<bool>(VideoRoomErrors.NotFound(id));
            await _repository.Delete(entity, cancellationToken);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir VideoRoom {Id}", id);
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
            _logger.LogError(ex, "Erro ao excluir video rooms em lote");
            return Result.Failure<bool>(Error.Failure(ex.Message));
        }
    }

    public async Task<Result<VideoRoomDTO?>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetById(id, cancellationToken);
        return entity is null ? Result.Failure<VideoRoomDTO?>(VideoRoomErrors.NotFound(id)) : Result.Success<VideoRoomDTO?>(entity.ToDTO());
    }

    public async Task<Result<IEnumerable<VideoRoomDTO?>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAll(cancellationToken);
        return Result.Success(entities.Select(x => x is null ? null : (VideoRoomDTO?)x.ToDTO()));
    }

    public async Task<Result<(IEnumerable<VideoRoomDTO?> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var filter = new VideoRoomFilterDTO(null, null, page, pageSize);
        var result = await GetByFilterAsync(filter, cancellationToken);
        if (!result.IsSuccess) return Result.Failure<(IEnumerable<VideoRoomDTO?> Items, int TotalCount)>(result.Error!);
        return Result.Success((result.Value!.Items.Cast<VideoRoomDTO?>(), result.Value.Pagination.TotalItems ?? 0));
    }

    public async Task<Result<IEnumerable<VideoRoomDTO?>>> FindAsync(Expression<Func<VideoRoomDTO, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAll(cancellationToken);
        var dtos = entities.Where(x => x is not null).Select(x => x!.ToDTO()).AsQueryable();
        var filtered = dtos.Where(predicate).ToList();
        return Result.Success(filtered.Cast<VideoRoomDTO?>());
    }

    public async Task<Result<int>> CountAsync(Expression<Func<VideoRoomDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAll(cancellationToken);
        var dtos = entities.Where(x => x is not null).Select(x => x!.ToDTO()).AsQueryable();
        var count = predicate is null ? dtos.Count() : dtos.Count(predicate);
        return Result.Success(count);
    }

    public async Task<Result<(IEnumerable<VideoRoomDTO> Items, PaginationData Pagination)>> GetByFilterAsync(VideoRoomFilterDTO filter, CancellationToken cancellationToken = default)
    {
        var queryFilter = new VideoRoomQueryFilter(filter.TenantId, filter.Search);
        var (items, total) = await _repository.FindByFilter(queryFilter, cancellationToken);
        var dtos = items.Select(x => x.ToDTO()).ToList();
        var pagination = new PaginationData(filter.Page, filter.PageSize, total, (int)Math.Ceiling(total / (double)(filter.PageSize ?? 20)));
        return Result.Success(((IEnumerable<VideoRoomDTO>)dtos, pagination));
    }
}
