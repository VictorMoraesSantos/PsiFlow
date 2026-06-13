using Agenda.Application.Contracts;
using Agenda.Application.DTOs.Appointment;
using Agenda.Application.Mapping;
using Agenda.Domain.Errors;
using Agenda.Domain.Events;
using Agenda.Domain.Filters;
using Agenda.Domain.Repositories;
using BuildingBlocks.Results;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Agenda.Application.Services
{
    public sealed class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repository;
        private readonly IAvailabilityRepository? _availabilityRepository;
        private readonly IScheduleBlockRepository? _scheduleBlockRepository;
        private readonly IAppointmentNotificationProvider? _notificationProvider;
        private readonly IAppointmentSessionProvider? _sessionProvider;
        private readonly IPatientRelationshipProvider? _patientRelationshipProvider;
        private readonly ILogger<AppointmentService> _logger;
        public AppointmentService(
            IAppointmentRepository repository,
            ILogger<AppointmentService> logger,
            IAvailabilityRepository? availabilityRepository = null,
            IScheduleBlockRepository? scheduleBlockRepository = null,
            IAppointmentNotificationProvider? notificationProvider = null,
            IAppointmentSessionProvider? sessionProvider = null,
            IPatientRelationshipProvider? patientRelationshipProvider = null)
        {
            _repository = repository;
            _availabilityRepository = availabilityRepository;
            _scheduleBlockRepository = scheduleBlockRepository;
            _notificationProvider = notificationProvider;
            _sessionProvider = sessionProvider;
            _patientRelationshipProvider = patientRelationshipProvider;
            _logger = logger;
        }

        public async Task<Result<int>> CreateAsync(CreateAppointmentDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result.Failure<int>(AppointmentErrors.CreateError);
                if (dto.EndsAt <= dto.StartsAt)
                    return Result.Failure<int>(AppointmentErrors.CreateError);
                if (_patientRelationshipProvider is not null)
                {
                    var relationship = await _patientRelationshipProvider.IsPatientLinkedToTenantAsync(dto.PatientId, dto.TenantId, cancellationToken);
                    if (!relationship.IsSuccess) return Result.Failure<int>(relationship.Error!);
                    if (!relationship.Value) return Result.Failure<int>(Error.Forbidden("Patient is not linked to tenant."));
                }
                if (_availabilityRepository is not null)
                {
                    var weekday = (int)dto.StartsAt.DayOfWeek;
                    var start = TimeOnly.FromDateTime(dto.StartsAt);
                    var end = TimeOnly.FromDateTime(dto.EndsAt);
                    var availabilities = await _availabilityRepository.Find(item => item.TenantId == dto.TenantId && item.IsActive && item.Weekday == weekday && item.Modality == dto.Modality && item.StartTime <= start && item.EndTime >= end, cancellationToken);
                    if (!availabilities.Any(item => item is not null)) return Result.Failure<int>(AppointmentErrors.OutsideAvailability);
                }
                if (_scheduleBlockRepository is not null)
                {
                    var blocks = await _scheduleBlockRepository.ListForPeriodAsync(dto.TenantId, dto.StartsAt, dto.EndsAt, cancellationToken);
                    if (blocks.Count > 0) return Result.Failure<int>(AppointmentErrors.BlockedByScheduleBlock);
                }
                var conflicts = await _repository.ListForPeriodAsync(dto.TenantId, dto.StartsAt, dto.EndsAt, "canceled", cancellationToken);
                if (conflicts.Any(item => item.PsychologistId == dto.PsychologistId)) return Result.Failure<int>(Error.Conflict("Appointment conflicts with existing appointment."));

                var entity = dto.ToEntity();
                var created = await _repository.CreateIfSlotIsFreeAsync(entity, cancellationToken);
                if (!created) return Result.Failure<int>(Error.Conflict("Appointment conflicts with existing appointment."));
                entity.AddDomainEvent(new AppointmentScheduledDomainEvent(entity.Id, entity.TenantId, entity.PatientId, entity.PsychologistId, entity.StartsAt, entity.EndsAt, entity.Modality));
                if (_notificationProvider is not null)
                {
                    var notification = await _notificationProvider.SendAppointmentScheduledAsync(new AppointmentScheduledNotification(entity.TenantId, entity.Id, entity.PatientId, entity.PsychologistId, entity.StartsAt, entity.EndsAt, entity.Modality), cancellationToken);
                    if (!notification.IsSuccess) return Result.Failure<int>(notification.Error!);
                }
                if (_sessionProvider is not null)
                {
                    var session = await _sessionProvider.CreateSessionForAppointmentAsync(new AppointmentSessionRequest(entity.TenantId, entity.Name, entity.Id, entity.PatientId, entity.PsychologistId, entity.StartsAt, entity.EndsAt, entity.Modality), cancellationToken);
                    if (!session.IsSuccess) return Result.Failure<int>(session.Error!);
                }
                return Result.Success(entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar Appointment");
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateAppointmentDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = dtos.Select(d => d.ToEntity()).ToList();
                await _repository.CreateRange(entities, cancellationToken);
                return Result.Success<IEnumerable<int>>(entities.Select(e => 0));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar appointments em lote");
                return Result.Failure<IEnumerable<int>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateAppointmentDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _repository.GetById(dto.Id, cancellationToken);
                if (entity is null) return Result.Failure<bool>(AppointmentErrors.NotFound(dto.Id));
                entity.TenantId = dto.TenantId;
                entity.Name = dto.Name;
                entity.PatientId = dto.PatientId;
                entity.PsychologistId = dto.PsychologistId;
                entity.StartsAt = dto.StartsAt;
                entity.EndsAt = dto.EndsAt;
                entity.Modality = dto.Modality;
                entity.Status = dto.Status;
                entity.MarkAsUpdated();
                await _repository.Update(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar Appointment {Id}", dto.Id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _repository.GetById(id, cancellationToken);
                if (entity is null) return Result.Failure<bool>(AppointmentErrors.NotFound(id));
                await _repository.Delete(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir Appointment {Id}", id);
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
                _logger.LogError(ex, "Erro ao excluir appointments em lote");
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<AppointmentDTO?>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetById(id, cancellationToken);
            return entity is null ? Result.Failure<AppointmentDTO?>(AppointmentErrors.NotFound(id)) : Result.Success<AppointmentDTO?>(entity.ToDTO());
        }

        public async Task<Result<IEnumerable<AppointmentDTO?>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAll(cancellationToken);
            return Result.Success(entities.Select(x => x is null ? null : (AppointmentDTO?)x.ToDTO()));
        }

        public async Task<Result<(IEnumerable<AppointmentDTO?> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var filter = new AppointmentFilterDTO(null, null, page, pageSize);
            var result = await GetByFilterAsync(filter, cancellationToken);
            if (!result.IsSuccess) return Result.Failure<(IEnumerable<AppointmentDTO?> Items, int TotalCount)>(result.Error!);
            return Result.Success((result.Value!.Items.Cast<AppointmentDTO?>(), result.Value.Pagination.TotalItems ?? 0));
        }

        public async Task<Result<IEnumerable<AppointmentDTO?>>> FindAsync(Expression<Func<AppointmentDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAll(cancellationToken);
            var dtos = entities.Where(x => x is not null).Select(x => x!.ToDTO()).AsQueryable();
            var filtered = dtos.Where(predicate).ToList();
            return Result.Success(filtered.Cast<AppointmentDTO?>());
        }

        public async Task<Result<int>> CountAsync(Expression<Func<AppointmentDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAll(cancellationToken);
            var dtos = entities.Where(x => x is not null).Select(x => x!.ToDTO()).AsQueryable();
            var count = predicate is null ? dtos.Count() : dtos.Count(predicate);
            return Result.Success(count);
        }

        public async Task<Result<(IEnumerable<AppointmentDTO> Items, PaginationData Pagination)>> GetByFilterAsync(AppointmentFilterDTO filter, CancellationToken cancellationToken = default)
        {
            var queryFilter = new AppointmentQueryFilter(filter.TenantId, filter.Search);
            var (items, total) = await _repository.FindByFilter(queryFilter, cancellationToken);
            var dtos = items.Select(x => x.ToDTO()).ToList();
            var pagination = new PaginationData(filter.Page, filter.PageSize, total, (int)Math.Ceiling(total / (double)(filter.PageSize ?? 20)));
            return Result.Success(((IEnumerable<AppointmentDTO>)dtos, pagination));
        }
    }
}
