using BuildingBlocks.Results;
using Microsoft.Extensions.Logging;
using Patients.Application.Contracts;
using Patients.Application.DTOs.Patient;
using Patients.Application.Mapping;
using Patients.Domain.Errors;
using Patients.Domain.Filters;
using Patients.Domain.Repositories;
using System.Linq.Expressions;

namespace Patients.Application.Services
{
    public sealed class PatientService : IPatientService
    {
        private readonly IPatientRepository _repository;
        private readonly ILogger<PatientService> _logger;
        public PatientService(IPatientRepository repository, ILogger<PatientService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<int>> CreateAsync(CreatePatientDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.FullName) || dto.FullName.Trim().Length < 2)
                    return Result.Failure<int>(PatientErrors.FullNameRequired);
                if (dto.FullName.Length > 160)
                    return Result.Failure<int>(PatientErrors.FullNameRequired);
                if (string.IsNullOrWhiteSpace(dto.Email))
                    return Result.Failure<int>(PatientErrors.EmailRequired);
                if (dto.BirthDate is { } bd && bd > DateOnly.FromDateTime(DateTime.UtcNow))
                    return Result.Failure<int>(PatientErrors.BirthDateInFuture);
                if (!string.IsNullOrWhiteSpace(dto.EmergencyContactName) && string.IsNullOrWhiteSpace(dto.EmergencyContactPhone))
                    return Result.Failure<int>(PatientErrors.EmergencyContactPhoneRequired);

                if (dto.TenantId > 0)
                {
                    var existing = await _repository.Find(x => x.TenantId == dto.TenantId && x.Email == dto.Email.Trim().ToLowerInvariant(), cancellationToken);
                    if (existing.Any()) return Result.Failure<int>(PatientErrors.DuplicateEmailInTenant);
                }

                var entity = dto.ToEntity();
                await _repository.Create(entity, cancellationToken);
                return Result.Success(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar Patient");
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreatePatientDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = dtos.Select(d => d.ToEntity()).ToList();
                await _repository.CreateRange(entities, cancellationToken);
                return Result.Success<IEnumerable<int>>(entities.Select(e => 0));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pacientes em lote");
                return Result.Failure<IEnumerable<int>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdatePatientDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _repository.GetById(dto.Id, cancellationToken);
                if (entity is null) return Result.Failure<bool>(PatientErrors.NotFound(dto.Id));
                entity.TenantId = dto.TenantId;
                entity.FullName = dto.FullName;
                entity.Email = dto.Email;
                entity.Phone = dto.Phone;
                entity.BirthDate = dto.BirthDate;
                entity.Status = dto.Status;
                entity.TreatmentStatus = dto.TreatmentStatus;
                entity.EmergencyContactName = dto.EmergencyContactName;
                entity.EmergencyContactPhone = dto.EmergencyContactPhone;
                entity.MarkAsUpdated();
                await _repository.Update(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar Patient {Id}", dto.Id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _repository.GetById(id, cancellationToken);
                if (entity is null) return Result.Failure<bool>(PatientErrors.NotFound(id));
                await _repository.Delete(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir Patient {Id}", id);
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
                _logger.LogError(ex, "Erro ao excluir pacientes em lote");
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<PatientDTO?>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetById(id, cancellationToken);
            return entity is null ? Result.Failure<PatientDTO?>(PatientErrors.NotFound(id)) : Result.Success<PatientDTO?>(entity.ToDTO());
        }

        public async Task<Result<IEnumerable<PatientDTO?>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAll(cancellationToken);
            return Result.Success(entities.Select(x => x is null ? null : (PatientDTO?)x.ToDTO()));
        }

        public async Task<Result<(IEnumerable<PatientDTO?> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var filter = new PatientFilterDTO(null, null, page, pageSize);
            var result = await GetByFilterAsync(filter, cancellationToken);
            if (!result.IsSuccess) return Result.Failure<(IEnumerable<PatientDTO?> Items, int TotalCount)>(result.Error!);
            return Result.Success((result.Value!.Items.Cast<PatientDTO?>(), result.Value.Pagination.TotalItems ?? 0));
        }

        public async Task<Result<IEnumerable<PatientDTO?>>> FindAsync(Expression<Func<PatientDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAll(cancellationToken);
            var dtos = entities.Where(x => x is not null).Select(x => x!.ToDTO()).AsQueryable();
            var filtered = dtos.Where(predicate).ToList();
            return Result.Success(filtered.Cast<PatientDTO?>());
        }

        public async Task<Result<int>> CountAsync(Expression<Func<PatientDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAll(cancellationToken);
            var dtos = entities.Where(x => x is not null).Select(x => x!.ToDTO()).AsQueryable();
            var count = predicate is null ? dtos.Count() : dtos.Count(predicate);
            return Result.Success(count);
        }

        public async Task<Result<(IEnumerable<PatientDTO> Items, PaginationData Pagination)>> GetByFilterAsync(PatientFilterDTO filter, CancellationToken cancellationToken = default)
        {
            var queryFilter = new PatientQueryFilter(filter.TenantId, filter.Search);
            var (items, total) = await _repository.FindByFilter(queryFilter, cancellationToken);
            var dtos = items.Select(x => x.ToDTO()).ToList();
            var pagination = new PaginationData(filter.Page, filter.PageSize, total, (int)Math.Ceiling(total / (double)(filter.PageSize ?? 20)));
            return Result.Success(((IEnumerable<PatientDTO>)dtos, pagination));
        }
    }
}
