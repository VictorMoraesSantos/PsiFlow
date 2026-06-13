using Agenda.Application.Contracts;
using Agenda.Domain.Entities;
using Agenda.Domain.Errors;
using Agenda.Domain.Events;
using Agenda.Domain.Repositories;
using BuildingBlocks.Results;

namespace Agenda.Application.Services;

public sealed class AgendaService(
    IAvailabilityRepository availabilityRepository,
    IScheduleBlockRepository scheduleBlockRepository,
    IAppointmentRepository appointmentRepository,
    IAppointmentNotificationProvider? notificationProvider = null,
    IAppointmentSessionProvider? sessionProvider = null) : IAgendaService
{
    public async Task<Result<WeeklyAvailabilityResult>> CreateWeeklyAvailabilityAsync(WeeklyAvailabilityRequest request, int tenantId, CancellationToken cancellationToken)
    {
        var validation = ValidateAvailabilityRequest(request);
        if (!validation.IsSuccess) return Result.Failure<WeeklyAvailabilityResult>(validation.Error!);
        var modality = NormalizeModality(request.Modality);
        if (await availabilityRepository.GetOverlappingAsync(tenantId, request.Weekday, modality, request.StartTime, request.EndTime, excludedId: null, cancellationToken))
        {
            return Result.Failure<WeeklyAvailabilityResult>(AppointmentErrors.OverlappingAvailability);
        }

        var availability = new Availability
        {
            TenantId = tenantId,
            Weekday = request.Weekday,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            SlotDurationMinutes = request.SlotDurationMinutes,
            Modality = modality,
            Timezone = NormalizeTimezone(request.Timezone),
            IsActive = request.IsActive
        };

        await availabilityRepository.Create(availability, cancellationToken);
        return Result.Success(ToResult(availability));
    }

    public async Task<Result<IReadOnlyCollection<WeeklyAvailabilityResult>>> GetWeeklyAvailabilitiesAsync(int tenantId, CancellationToken cancellationToken)
    {
        var items = await availabilityRepository.Find(item => item.TenantId == tenantId, cancellationToken);
        var ordered = items
            .Where(item => item is not null)
            .Select(item => item!)
            .OrderBy(item => item.Weekday)
            .ThenBy(item => item.StartTime)
            .Select(ToResult)
            .ToList();
        return Result.Success<IReadOnlyCollection<WeeklyAvailabilityResult>>(ordered);
    }

    public async Task<Result<bool>> UpdateWeeklyAvailabilityAsync(int availabilityId, WeeklyAvailabilityRequest request, int tenantId, CancellationToken cancellationToken)
    {
        var validation = ValidateAvailabilityRequest(request);
        if (!validation.IsSuccess) return Result.Failure<bool>(validation.Error!);
        var availability = await availabilityRepository.GetById(availabilityId, cancellationToken);
        if (availability is null || availability.TenantId != tenantId) return Result.Failure<bool>(Error.NotFound("Availability not found"));

        var modality = NormalizeModality(request.Modality ?? availability.Modality);
        if (await availabilityRepository.GetOverlappingAsync(tenantId, request.Weekday, modality, request.StartTime, request.EndTime, availabilityId, cancellationToken))
        {
            return Result.Failure<bool>(AppointmentErrors.OverlappingAvailability);
        }

        availability.Weekday = request.Weekday;
        availability.StartTime = request.StartTime;
        availability.EndTime = request.EndTime;
        availability.SlotDurationMinutes = request.SlotDurationMinutes;
        availability.Modality = modality;
        availability.Timezone = NormalizeTimezone(request.Timezone ?? availability.Timezone);
        availability.IsActive = request.IsActive;
        await availabilityRepository.Update(availability, cancellationToken);
        return Result.Success(true);
    }

    public async Task<Result<IReadOnlyCollection<WeeklyAvailabilityResult>>> ReplaceWeeklyAvailabilitiesAsync(WeeklyAvailabilityReplaceRequest request, int tenantId, CancellationToken cancellationToken)
    {
        var items = request.Items ?? Array.Empty<WeeklyAvailabilityRequest>();
        foreach (var item in items)
        {
            var validation = ValidateAvailabilityRequest(item);
            if (!validation.IsSuccess) return Result.Failure<IReadOnlyCollection<WeeklyAvailabilityResult>>(validation.Error!);
        }

        var normalized = items
            .Select(item => new Availability
            {
                TenantId = tenantId,
                Weekday = item.Weekday,
                StartTime = item.StartTime,
                EndTime = item.EndTime,
                SlotDurationMinutes = item.SlotDurationMinutes,
                Modality = NormalizeModality(item.Modality),
                Timezone = NormalizeTimezone(item.Timezone),
                IsActive = item.IsActive
            })
            .ToList();

        var hasOverlap = normalized
            .GroupBy(item => new { item.Weekday, item.Modality })
            .Any(group => group.Any(left => group.Any(right => !ReferenceEquals(left, right) && left.StartTime < right.EndTime && left.EndTime > right.StartTime)));
        if (hasOverlap) return Result.Failure<IReadOnlyCollection<WeeklyAvailabilityResult>>(AppointmentErrors.OverlappingAvailability);

        await availabilityRepository.ReplaceTenantWeekAsync(tenantId, normalized, cancellationToken);
        return Result.Success<IReadOnlyCollection<WeeklyAvailabilityResult>>(normalized.OrderBy(item => item.Weekday).ThenBy(item => item.StartTime).Select(ToResult).ToList());
    }

    public async Task<Result<bool>> DeleteWeeklyAvailabilityAsync(int availabilityId, int tenantId, CancellationToken cancellationToken)
    {
        var availability = await availabilityRepository.GetById(availabilityId, cancellationToken);
        if (availability is null || availability.TenantId != tenantId) return Result.Failure<bool>(Error.NotFound("Availability not found"));

        await availabilityRepository.Delete(availability, cancellationToken);
        return Result.Success(true);
    }

    public async Task<Result<ScheduleBlockResult>> CreateScheduleBlockAsync(ScheduleBlockRequest request, int tenantId, int userId, CancellationToken cancellationToken)
    {
        if (await scheduleBlockRepository.ExistsForPeriodAsync(tenantId, request.StartsAt, request.EndsAt, cancellationToken))
        {
            return Result.Failure<ScheduleBlockResult>(Error.Conflict("Schedule block already exists"));
        }

        var block = new ScheduleBlock { TenantId = tenantId, StartsAt = request.StartsAt, EndsAt = request.EndsAt, Reason = request.Reason, CreatedBy = userId };
        await scheduleBlockRepository.Create(block, cancellationToken);
        var affectedAppointments = await appointmentRepository.ListForPeriodAsync(tenantId, request.StartsAt, request.EndsAt, "canceled", cancellationToken);
        var affectedIds = affectedAppointments.Select(appointment => appointment.Id).ToList();
        if (affectedIds.Count > 0)
        {
            block.AddDomainEvent(new ScheduleBlockAffectsAppointmentsDomainEvent(block.Id, tenantId, block.StartsAt, block.EndsAt, affectedIds));
            await scheduleBlockRepository.Update(block, cancellationToken);
        }
        return Result.Success(new ScheduleBlockResult(block.Id, block.StartsAt, block.EndsAt, block.Reason));
    }

    public async Task<Result<IReadOnlyCollection<ScheduleBlockResult>>> GetScheduleBlocksAsync(int tenantId, CancellationToken cancellationToken)
    {
        var items = await scheduleBlockRepository.Find(item => item.TenantId == tenantId, cancellationToken);
        var ordered = items
            .Where(item => item is not null)
            .Select(item => item!)
            .OrderBy(item => item.StartsAt)
            .Select(item => new ScheduleBlockResult(item.Id, item.StartsAt, item.EndsAt, item.Reason))
            .ToList();
        return Result.Success<IReadOnlyCollection<ScheduleBlockResult>>(ordered);
    }

    public async Task<Result<bool>> DeleteScheduleBlockAsync(int blockId, int tenantId, CancellationToken cancellationToken)
    {
        var block = await scheduleBlockRepository.GetById(blockId, cancellationToken);
        if (block is null || block.TenantId != tenantId) return Result.Failure<bool>(Error.NotFound("Schedule block not found"));

        await scheduleBlockRepository.Delete(block, cancellationToken);
        return Result.Success(true);
    }

    public async Task<Result<IReadOnlyCollection<AvailableSlotResult>>> GetAvailableSlotsAsync(AvailableSlotsRequest request, int tenantId, CancellationToken cancellationToken)
    {
        var availabilityFilter = await availabilityRepository.Find(
            item => item.TenantId == tenantId && item.IsActive && (request.Modality == null || item.Modality == request.Modality),
            cancellationToken);
        var availabilities = availabilityFilter.Where(item => item is not null).Select(item => item!).ToList();
        var blocks = (await scheduleBlockRepository.ListForPeriodAsync(tenantId, request.From, request.To, cancellationToken)).ToList();
        var appointments = (await appointmentRepository.ListForPeriodAsync(tenantId, request.From, request.To, "canceled", cancellationToken)).ToList();

        var slots = new List<AvailableSlotResult>();
        for (var date = request.From.Date; date <= request.To.Date; date = date.AddDays(1))
        {
            foreach (var availability in availabilities.Where(item => item.Weekday == (int)date.DayOfWeek))
            {
                var start = date.Add(availability.StartTime.ToTimeSpan());
                var end = date.Add(availability.EndTime.ToTimeSpan());
                for (var slotStart = start; slotStart.AddMinutes(availability.SlotDurationMinutes) <= end; slotStart = slotStart.AddMinutes(availability.SlotDurationMinutes))
                {
                    var slotEnd = slotStart.AddMinutes(availability.SlotDurationMinutes);
                    if (slotStart < request.From || slotEnd > request.To) continue;
                    if (blocks.Any(item => item.StartsAt < slotEnd && item.EndsAt > slotStart)) continue;
                    if (appointments.Any(item => item.StartsAt < slotEnd && item.EndsAt > slotStart)) continue;
                    slots.Add(new AvailableSlotResult(slotStart, slotEnd, availability.Modality));
                }
            }
        }

        return Result.Success<IReadOnlyCollection<AvailableSlotResult>>(slots);
    }

    public async Task<Result<IReadOnlyCollection<AgendaCalendarItemResult>>> GetAgendaAsync(DateTime from, DateTime to, int tenantId, int? patientId, CancellationToken cancellationToken)
    {
        if (to <= from) return Result.Failure<IReadOnlyCollection<AgendaCalendarItemResult>>(Error.Failure("Invalid agenda period."));
        var appointments = await appointmentRepository.ListForPeriodAsync(tenantId, from, to, "canceled", cancellationToken);
        var items = appointments
            .Where(item => patientId is null || item.PatientId == patientId)
            .OrderBy(item => item.StartsAt)
            .Select(item => new AgendaCalendarItemResult(item.Id, item.PatientId, item.PsychologistId, item.StartsAt, item.EndsAt, item.Status, item.Modality, OnlineSessionLink: null))
            .ToList();
        return Result.Success<IReadOnlyCollection<AgendaCalendarItemResult>>(items);
    }

    public async Task<Result<bool>> CancelAppointmentAsync(int appointmentId, CancelAppointmentRequest request, int tenantId, int userId, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetByIdAndTenantAsync(appointmentId, tenantId, cancellationToken);
        if (appointment is null) return Result.Failure<bool>(Error.NotFound("Appointment not found"));
        if (appointment.Status == "canceled") return Result.Failure<bool>(AppointmentErrors.AlreadyCancelled);
        if (appointment.StartsAt <= DateTime.UtcNow) return Result.Failure<bool>(AppointmentErrors.CannotCancelPast);

        var canceledAt = DateTime.UtcNow;
        appointment.Status = "canceled";
        appointment.CanceledAt = canceledAt;
        appointment.CanceledBy = userId;
        appointment.CancelReason = request.Reason;
        appointment.LateCancel = appointment.StartsAt - canceledAt < TimeSpan.FromHours(24);
        appointment.AddDomainEvent(new AppointmentCancelledDomainEvent(appointment.Id, appointment.TenantId, appointment.PatientId, appointment.PsychologistId, canceledAt, userId, request.Reason, appointment.LateCancel));
        await appointmentRepository.Update(appointment, cancellationToken);
        if (sessionProvider is not null)
        {
            var session = await sessionProvider.CancelSessionForAppointmentAsync(tenantId, appointmentId, request.Reason, cancellationToken);
            if (!session.IsSuccess) return Result.Failure<bool>(session.Error!);
        }
        return Result.Success(true);
    }

    public async Task<Result<RescheduleAppointmentResult>> RescheduleAppointmentAsync(int appointmentId, RescheduleAppointmentRequest request, int tenantId, int userId, CancellationToken cancellationToken)
    {
        var previous = await appointmentRepository.GetByIdAndTenantAsync(appointmentId, tenantId, cancellationToken);
        if (previous is null) return Result.Failure<RescheduleAppointmentResult>(Error.NotFound("Appointment not found"));
        if (previous.Status == "canceled") return Result.Failure<RescheduleAppointmentResult>(AppointmentErrors.AlreadyCancelled);
        if (request.EndsAt <= request.StartsAt) return Result.Failure<RescheduleAppointmentResult>(AppointmentErrors.CreateError);

        var modality = string.IsNullOrWhiteSpace(request.Modality) ? previous.Modality : request.Modality.Trim();
        var weekday = (int)request.StartsAt.DayOfWeek;
        var start = TimeOnly.FromDateTime(request.StartsAt);
        var end = TimeOnly.FromDateTime(request.EndsAt);
        var availabilities = await availabilityRepository.Find(item => item.TenantId == tenantId && item.IsActive && item.Weekday == weekday && item.Modality == modality && item.StartTime <= start && item.EndTime >= end, cancellationToken);
        if (!availabilities.Any(item => item is not null)) return Result.Failure<RescheduleAppointmentResult>(AppointmentErrors.OutsideAvailability);
        var blocks = await scheduleBlockRepository.ListForPeriodAsync(tenantId, request.StartsAt, request.EndsAt, cancellationToken);
        if (blocks.Count > 0) return Result.Failure<RescheduleAppointmentResult>(AppointmentErrors.BlockedByScheduleBlock);
        var conflicts = await appointmentRepository.ListForPeriodAsync(tenantId, request.StartsAt, request.EndsAt, "canceled", cancellationToken);
        if (conflicts.Any(item => item.Id != previous.Id && item.PsychologistId == previous.PsychologistId)) return Result.Failure<RescheduleAppointmentResult>(Error.Conflict("Appointment conflicts with existing appointment."));

        var canceledAt = DateTime.UtcNow;
        previous.Status = "canceled";
        previous.CanceledAt = canceledAt;
        previous.CanceledBy = userId;
        previous.CancelReason = request.Reason ?? "Rescheduled";
        previous.LateCancel = previous.StartsAt - canceledAt < TimeSpan.FromHours(24);

        var replacement = new Appointment
        {
            TenantId = tenantId,
            Name = previous.Name,
            PatientId = previous.PatientId,
            PsychologistId = previous.PsychologistId,
            StartsAt = request.StartsAt,
            EndsAt = request.EndsAt,
            Modality = modality,
            Status = "scheduled",
            CreatedBy = userId
        };

        await appointmentRepository.Update(previous, cancellationToken);
        var created = await appointmentRepository.CreateIfSlotIsFreeAsync(replacement, cancellationToken);
        if (!created) return Result.Failure<RescheduleAppointmentResult>(Error.Conflict("Appointment conflicts with existing appointment."));

        replacement.AddDomainEvent(new AppointmentRescheduledDomainEvent(previous.Id, replacement.Id, tenantId, replacement.PatientId, replacement.PsychologistId));
        if (notificationProvider is not null)
        {
            var notification = await notificationProvider.SendAppointmentScheduledAsync(new AppointmentScheduledNotification(replacement.TenantId, replacement.Id, replacement.PatientId, replacement.PsychologistId, replacement.StartsAt, replacement.EndsAt, replacement.Modality), cancellationToken);
            if (!notification.IsSuccess) return Result.Failure<RescheduleAppointmentResult>(notification.Error!);
        }
        if (sessionProvider is not null)
        {
            var session = await sessionProvider.CreateSessionForAppointmentAsync(new AppointmentSessionRequest(replacement.TenantId, replacement.Name, replacement.Id, replacement.PatientId, replacement.PsychologistId, replacement.StartsAt, replacement.EndsAt, replacement.Modality), cancellationToken);
            if (!session.IsSuccess) return Result.Failure<RescheduleAppointmentResult>(session.Error!);
        }

        return Result.Success(new RescheduleAppointmentResult(previous.Id, replacement.Id));
    }

    private static WeeklyAvailabilityResult ToResult(Availability availability) =>
        new(availability.Id, availability.Weekday, availability.StartTime, availability.EndTime, availability.SlotDurationMinutes, availability.Modality, availability.Timezone, availability.IsActive);

    private static Result<bool> ValidateAvailabilityRequest(WeeklyAvailabilityRequest request)
    {
        if (request.StartTime >= request.EndTime) return Result.Failure<bool>(Error.Failure("StartTime must be before EndTime."));
        if (request.SlotDurationMinutes is not (30 or 45 or 50 or 60)) return Result.Failure<bool>(Error.Failure("SlotDurationMinutes is not allowed."));
        if (request.Weekday is < 0 or > 6) return Result.Failure<bool>(Error.Failure("Weekday must be between 0 and 6."));
        return Result.Success(true);
    }

    private static string NormalizeModality(string? modality) => string.IsNullOrWhiteSpace(modality) ? "online" : modality.Trim();
    private static string NormalizeTimezone(string? timezone) => string.IsNullOrWhiteSpace(timezone) ? "America/Sao_Paulo" : timezone.Trim();
}
