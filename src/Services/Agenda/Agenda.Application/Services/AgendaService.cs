using Agenda.Application.Contracts;
using Agenda.Domain.Entities;
using Agenda.Domain.Errors;
using Agenda.Domain.Repositories;
using BuildingBlocks.Results;

namespace Agenda.Application.Services;

public sealed class AgendaService(
    IAvailabilityRepository availabilityRepository,
    IScheduleBlockRepository scheduleBlockRepository,
    IAppointmentRepository appointmentRepository) : IAgendaService
{
    public async Task<Result<WeeklyAvailabilityResult>> CreateWeeklyAvailabilityAsync(WeeklyAvailabilityRequest request, int tenantId, CancellationToken cancellationToken)
    {
        var modality = request.Modality ?? "online";
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
            Timezone = request.Timezone ?? "America/Sao_Paulo",
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
        var availability = await availabilityRepository.GetById(availabilityId, cancellationToken);
        if (availability is null || availability.TenantId != tenantId) return Result.Failure<bool>(Error.NotFound("Availability not found"));

        var modality = request.Modality ?? availability.Modality;
        if (await availabilityRepository.GetOverlappingAsync(tenantId, request.Weekday, modality, request.StartTime, request.EndTime, availabilityId, cancellationToken))
        {
            return Result.Failure<bool>(AppointmentErrors.OverlappingAvailability);
        }

        availability.Weekday = request.Weekday;
        availability.StartTime = request.StartTime;
        availability.EndTime = request.EndTime;
        availability.SlotDurationMinutes = request.SlotDurationMinutes;
        availability.Modality = modality;
        availability.Timezone = request.Timezone ?? availability.Timezone;
        availability.IsActive = request.IsActive;
        await availabilityRepository.Update(availability, cancellationToken);
        return Result.Success(true);
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
        await appointmentRepository.Update(appointment, cancellationToken);
        return Result.Success(true);
    }

    private static WeeklyAvailabilityResult ToResult(Availability availability) =>
        new(availability.Id, availability.Weekday, availability.StartTime, availability.EndTime, availability.SlotDurationMinutes, availability.Modality, availability.Timezone, availability.IsActive);
}
