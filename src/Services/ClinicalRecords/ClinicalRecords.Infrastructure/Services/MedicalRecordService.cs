using BuildingBlocks.Results;
using ClinicalRecords.Application.Contracts;
using ClinicalRecords.Application.DTOs.MedicalRecord;
using ClinicalRecords.Application.Mapping;
using ClinicalRecords.Domain.Errors;
using ClinicalRecords.Domain.Filters;
using ClinicalRecords.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace ClinicalRecords.Infrastructure.Services
{
    public sealed class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _repository;
        private readonly ILogger<MedicalRecordService> _logger;
        public MedicalRecordService(IMedicalRecordRepository repository, ILogger<MedicalRecordService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<int>> CreateAsync(CreateMedicalRecordDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result.Failure<int>(MedicalRecordErrors.CreateError);

                var entity = dto.ToEntity();
                await _repository.Create(entity, cancellationToken);
                return Result.Success(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar MedicalRecord");
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateMedicalRecordDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = dtos.Select(d => d.ToEntity()).ToList();
                await _repository.CreateRange(entities, cancellationToken);
                return Result.Success<IEnumerable<int>>(entities.Select(e => 0));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar medical records em lote");
                return Result.Failure<IEnumerable<int>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateMedicalRecordDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _repository.GetById(dto.Id, cancellationToken);
                if (entity is null) return Result.Failure<bool>(MedicalRecordErrors.NotFound(dto.Id));
                entity.TenantId = dto.TenantId;
                entity.PatientId = dto.PatientId;
                entity.Name = dto.Name;
                entity.Status = dto.Status;
                entity.MarkAsUpdated();
                await _repository.Update(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar MedicalRecord {Id}", dto.Id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _repository.GetById(id, cancellationToken);
                if (entity is null) return Result.Failure<bool>(MedicalRecordErrors.NotFound(id));
                await _repository.Delete(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir MedicalRecord {Id}", id);
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
                _logger.LogError(ex, "Erro ao excluir medical records em lote");
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<MedicalRecordDTO?>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetById(id, cancellationToken);
            return entity is null ? Result.Failure<MedicalRecordDTO?>(MedicalRecordErrors.NotFound(id)) : Result.Success<MedicalRecordDTO?>(entity.ToDTO());
        }

        public async Task<Result<IEnumerable<MedicalRecordDTO?>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAll(cancellationToken);
            return Result.Success(entities.Select(x => x is null ? null : (MedicalRecordDTO?)x.ToDTO()));
        }

        public async Task<Result<(IEnumerable<MedicalRecordDTO?> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var filter = new MedicalRecordFilterDTO(null, null, page, pageSize);
            var result = await GetByFilterAsync(filter, cancellationToken);
            if (!result.IsSuccess) return Result.Failure<(IEnumerable<MedicalRecordDTO?> Items, int TotalCount)>(result.Error!);
            return Result.Success((result.Value!.Items.Cast<MedicalRecordDTO?>(), result.Value.Pagination.TotalItems ?? 0));
        }

        public async Task<Result<IEnumerable<MedicalRecordDTO?>>> FindAsync(Expression<Func<MedicalRecordDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAll(cancellationToken);
            var dtos = entities.Where(x => x is not null).Select(x => x!.ToDTO()).AsQueryable();
            var filtered = dtos.Where(predicate).ToList();
            return Result.Success(filtered.Cast<MedicalRecordDTO?>());
        }

        public async Task<Result<int>> CountAsync(Expression<Func<MedicalRecordDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAll(cancellationToken);
            var dtos = entities.Where(x => x is not null).Select(x => x!.ToDTO()).AsQueryable();
            var count = predicate is null ? dtos.Count() : dtos.Count(predicate);
            return Result.Success(count);
        }

        public async Task<Result<(IEnumerable<MedicalRecordDTO> Items, PaginationData Pagination)>> GetByFilterAsync(MedicalRecordFilterDTO filter, CancellationToken cancellationToken = default)
        {
            var queryFilter = new MedicalRecordQueryFilter(filter.TenantId, filter.Search);
            var (items, total) = await _repository.FindByFilter(queryFilter, cancellationToken);
            var dtos = items.Select(x => x.ToDTO()).ToList();
            var pagination = new PaginationData(filter.Page, filter.PageSize, total, (int)Math.Ceiling(total / (double)(filter.PageSize ?? 20)));
            return Result.Success(((IEnumerable<MedicalRecordDTO>)dtos, pagination));
        }
    }
}
