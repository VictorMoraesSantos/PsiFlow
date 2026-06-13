using BuildingBlocks.Results;
using Microsoft.Extensions.Logging;
using Patients.Application.Contracts;
using Patients.Application.DTOs.Patient;
using Patients.Application.Mapping;
using Patients.Domain.Errors;
using Patients.Domain.Events;
using Patients.Domain.Filters;
using Patients.Domain.Repositories;
using System.Linq.Expressions;
using System.Net.Mail;

namespace Patients.Application.Services
{
    public sealed class PatientService : IPatientService
    {
        private readonly IPatientRepository _repository;
        private readonly IPatientAdministrativeNoteRepository? _administrativeNoteRepository;
        private readonly IPatientStatusHistoryRepository? _statusHistoryRepository;
        private readonly IPatientSessionsProvider? _sessionsProvider;
        private readonly ILogger<PatientService> _logger;
        public PatientService(
            IPatientRepository repository,
            ILogger<PatientService> logger,
            IPatientAdministrativeNoteRepository? administrativeNoteRepository = null,
            IPatientStatusHistoryRepository? statusHistoryRepository = null,
            IPatientSessionsProvider? sessionsProvider = null)
        {
            _repository = repository;
            _administrativeNoteRepository = administrativeNoteRepository;
            _statusHistoryRepository = statusHistoryRepository;
            _sessionsProvider = sessionsProvider;
            _logger = logger;
        }

        public async Task<Result<int>> CreateAsync(CreatePatientDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                dto = Normalize(dto);
                if (string.IsNullOrWhiteSpace(dto.FullName) || dto.FullName.Trim().Length < 2)
                    return Result.Failure<int>(PatientErrors.FullNameRequired);
                if (dto.FullName.Length > 160)
                    return Result.Failure<int>(PatientErrors.FullNameRequired);
                if (string.IsNullOrWhiteSpace(dto.Email))
                    return Result.Failure<int>(PatientErrors.EmailRequired);
                if (!IsValidEmail(dto.Email))
                    return Result.Failure<int>(PatientErrors.EmailInvalid);
                if (string.IsNullOrWhiteSpace(dto.Phone))
                    return Result.Failure<int>(PatientErrors.PhoneRequired);
                if (dto.BirthDate is { } bd && bd > DateOnly.FromDateTime(DateTime.UtcNow))
                    return Result.Failure<int>(PatientErrors.BirthDateInFuture);
                if (!string.IsNullOrWhiteSpace(dto.EmergencyContactName) && string.IsNullOrWhiteSpace(dto.EmergencyContactPhone))
                    return Result.Failure<int>(PatientErrors.EmergencyContactPhoneRequired);
                if (IsMinor(dto.BirthDate) && (string.IsNullOrWhiteSpace(dto.EmergencyContactName) || string.IsNullOrWhiteSpace(dto.EmergencyContactPhone)))
                    return Result.Failure<int>(PatientErrors.ResponsibleLegalRequiredForMinor);

                if (dto.TenantId > 0)
                {
                    var existing = await _repository.Find(x => x.TenantId == dto.TenantId && x.Email == dto.Email && x.Status == "active", cancellationToken);
                    if (existing.Any()) return Result.Failure<int>(PatientErrors.DuplicateEmailInTenant);
                }

                var entity = dto.ToEntity();
                await _repository.Create(entity, cancellationToken);
                entity.AddDomainEvent(new PatientCreatedDomainEvent(entity.Id, entity.TenantId, entity.FullName, entity.Email));
                return Result.Success(entity.Id);
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
                dto = Normalize(dto);
                if (string.IsNullOrWhiteSpace(dto.FullName) || dto.FullName.Length < 2 || dto.FullName.Length > 160)
                    return Result.Failure<bool>(PatientErrors.FullNameRequired);
                if (string.IsNullOrWhiteSpace(dto.Email))
                    return Result.Failure<bool>(PatientErrors.EmailRequired);
                if (!IsValidEmail(dto.Email))
                    return Result.Failure<bool>(PatientErrors.EmailInvalid);
                if (string.IsNullOrWhiteSpace(dto.Phone))
                    return Result.Failure<bool>(PatientErrors.PhoneRequired);
                if (dto.BirthDate is { } bd && bd > DateOnly.FromDateTime(DateTime.UtcNow))
                    return Result.Failure<bool>(PatientErrors.BirthDateInFuture);
                if (!string.IsNullOrWhiteSpace(dto.EmergencyContactName) && string.IsNullOrWhiteSpace(dto.EmergencyContactPhone))
                    return Result.Failure<bool>(PatientErrors.EmergencyContactPhoneRequired);
                if (IsMinor(dto.BirthDate) && (string.IsNullOrWhiteSpace(dto.EmergencyContactName) || string.IsNullOrWhiteSpace(dto.EmergencyContactPhone)))
                    return Result.Failure<bool>(PatientErrors.ResponsibleLegalRequiredForMinor);
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
                entity.Address = dto.Address;
                entity.DocumentNumber = dto.DocumentNumber;
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

        public async Task<Result<PatientDTO?>> GetByIdAndTenantAsync(int id, int tenantId, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetById(id, cancellationToken);
            if (entity is null) return Result.Failure<PatientDTO?>(PatientErrors.NotFound(id));
            if (entity.TenantId != tenantId) return Result.Failure<PatientDTO?>(Error.Forbidden("Patient belongs to another tenant."));
            return Result.Success<PatientDTO?>(entity.ToDTO());
        }

        public async Task<Result<IEnumerable<PatientDTO?>>> ListByTenantAsync(int tenantId, CancellationToken cancellationToken = default)
        {
            var entities = await _repository.Find(x => x.TenantId == tenantId, cancellationToken);
            return Result.Success<IEnumerable<PatientDTO?>>(entities.Select(x => x is null ? null : (PatientDTO?)x.ToDTO()));
        }

        public async Task<Result<PatientAdministrativeProfileDTO>> GetAdministrativeProfileAsync(int id, int tenantId, CancellationToken cancellationToken = default)
        {
            var patientResult = await GetByIdAndTenantAsync(id, tenantId, cancellationToken);
            if (!patientResult.IsSuccess) return Result.Failure<PatientAdministrativeProfileDTO>(patientResult.Error!);

            var notes = _administrativeNoteRepository is null
                ? Enumerable.Empty<PatientAdministrativeNoteDTO>()
                : (await _administrativeNoteRepository.Find(x => x.PatientId == id && x.TenantId == tenantId, cancellationToken))
                    .Where(x => x is not null)
                    .OrderByDescending(x => x!.CreatedAt)
                    .Select(x => new PatientAdministrativeNoteDTO(x!.Id, x.PatientId, x.Text, x.CreatedBy, x.CreatedAt));

            var timeline = _statusHistoryRepository is null
                ? Enumerable.Empty<PatientStatusTimelineItemDTO>()
                : (await _statusHistoryRepository.Find(x => x.PatientId == id && x.TenantId == tenantId, cancellationToken))
                    .Where(x => x is not null)
                    .OrderByDescending(x => x!.CreatedAt)
                    .Select(x => new PatientStatusTimelineItemDTO(x!.Id, x.PatientId, x.FromStatus, x.ToStatus, x.Reason, x.ChangedBy, x.CreatedAt));

            var sessionsResult = _sessionsProvider is null
                ? Result.Success<IReadOnlyCollection<PatientSessionHistoryDTO>>(Array.Empty<PatientSessionHistoryDTO>())
                : await _sessionsProvider.GetPatientSessionsAsync(id, tenantId, cancellationToken);
            if (!sessionsResult.IsSuccess) return Result.Failure<PatientAdministrativeProfileDTO>(sessionsResult.Error!);
            var sessions = sessionsResult.Value ?? Array.Empty<PatientSessionHistoryDTO>();
            var summary = new { patientId = id, totalSessions = sessions.Count, completedSessions = sessions.Count(x => x.Status == "completed"), noShows = sessions.Count(x => x.Status == "no_show") };

            return Result.Success(new PatientAdministrativeProfileDTO(patientResult.Value!, notes, timeline, summary, sessions));
        }

        public async Task<Result<PatientDTO>> PatchAdministrativeProfileAsync(int id, int tenantId, PatchPatientAdministrativeDTO dto, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetById(id, cancellationToken);
            if (entity is null) return Result.Failure<PatientDTO>(PatientErrors.NotFound(id));
            if (entity.TenantId != tenantId) return Result.Failure<PatientDTO>(Error.Forbidden("Patient belongs to another tenant."));

            var updated = new UpdatePatientDTO(
                entity.Id,
                entity.TenantId,
                dto.FullName ?? entity.FullName,
                dto.Email ?? entity.Email,
                dto.Phone ?? entity.Phone,
                dto.BirthDate ?? entity.BirthDate,
                entity.Status,
                entity.TreatmentStatus,
                dto.EmergencyContactName ?? entity.EmergencyContactName,
                dto.EmergencyContactPhone ?? entity.EmergencyContactPhone,
                dto.Address ?? entity.Address,
                dto.DocumentNumber ?? entity.DocumentNumber);

            var result = await UpdateAsync(updated, cancellationToken);
            return result.IsSuccess ? Result.Success(entity.ToDTO()) : Result.Failure<PatientDTO>(result.Error!);
        }

        public async Task<Result<PatientAdministrativeNoteDTO>> AddAdministrativeNoteAsync(int id, int tenantId, int userId, CreatePatientAdministrativeNoteDTO dto, CancellationToken cancellationToken = default)
        {
            if (_administrativeNoteRepository is null) return Result.Failure<PatientAdministrativeNoteDTO>(Error.Failure("Administrative notes repository is not configured."));
            if (string.IsNullOrWhiteSpace(dto.Text)) return Result.Failure<PatientAdministrativeNoteDTO>(Error.Failure("Administrative note text is required."));

            var entity = await _repository.GetById(id, cancellationToken);
            if (entity is null) return Result.Failure<PatientAdministrativeNoteDTO>(PatientErrors.NotFound(id));
            if (entity.TenantId != tenantId) return Result.Failure<PatientAdministrativeNoteDTO>(Error.Forbidden("Patient belongs to another tenant."));

            var note = new Patients.Domain.Entities.PatientAdministrativeNote
            {
                TenantId = tenantId,
                PatientId = id,
                Text = dto.Text.Trim(),
                CreatedBy = userId
            };

            await _administrativeNoteRepository.Create(note, cancellationToken);
            return Result.Success(new PatientAdministrativeNoteDTO(note.Id, note.PatientId, note.Text, note.CreatedBy, note.CreatedAt));
        }

        public Task<Result<bool>> DeleteAsync(int id, int tenantId, CancellationToken cancellationToken = default) =>
            DeleteInternalAsync(id, tenantId, cancellationToken);

        private async Task<Result<bool>> DeleteInternalAsync(int id, int tenantId, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _repository.GetById(id, cancellationToken);
                if (entity is null) return Result.Failure<bool>(PatientErrors.NotFound(id));
                if (entity.TenantId != tenantId) return Result.Failure<bool>(Error.Forbidden("Patient belongs to another tenant."));
                await _repository.Delete(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir Patient {Id}", id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
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

        private static CreatePatientDTO Normalize(CreatePatientDTO dto) => dto with
        {
            FullName = NormalizeText(dto.FullName),
            Email = NormalizeEmail(dto.Email),
            Phone = NormalizePhone(dto.Phone),
            EmergencyContactName = NormalizeNullableText(dto.EmergencyContactName),
            EmergencyContactPhone = NormalizeNullablePhone(dto.EmergencyContactPhone)
        };

        private static UpdatePatientDTO Normalize(UpdatePatientDTO dto) => dto with
        {
            FullName = NormalizeText(dto.FullName),
            Email = NormalizeEmail(dto.Email),
            Phone = NormalizePhone(dto.Phone),
            EmergencyContactName = NormalizeNullableText(dto.EmergencyContactName),
            EmergencyContactPhone = NormalizeNullablePhone(dto.EmergencyContactPhone)
        };

        private static string NormalizeText(string value) => value.Trim();
        private static string NormalizeEmail(string value) => value.Trim().ToLowerInvariant();
        private static string NormalizePhone(string value) => new(value.Where(char.IsDigit).ToArray());
        private static string? NormalizeNullableText(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        private static string? NormalizeNullablePhone(string? value) => string.IsNullOrWhiteSpace(value) ? null : new string(value.Where(char.IsDigit).ToArray());
        private static bool IsValidEmail(string value) => MailAddress.TryCreate(value, out _);
        private static bool IsMinor(DateOnly? birthDate)
        {
            if (birthDate is null) return false;
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - birthDate.Value.Year;
            if (birthDate.Value > today.AddYears(-age)) age--;
            return age < 18;
        }
    }
}
