using Agenda.Application.Contracts;
using Agenda.Domain.Aggregates;
using BuildingBlocks.Results;
using Microsoft.EntityFrameworkCore;
using PsiFlow.Agenda.Infrastructure.Persistence;

namespace Agenda.Infrastructure.Services;

public sealed class AgendaService(AgendaDbContext dbContext) : IAgendaService
{
    public async Task<Result<WeeklyAvailabilityResult>> CreateWeeklyAvailabilityAsync(WeeklyAvailabilityRequest request, int tenantId, CancellationToken cancellationToken)
    {
        var availability = new Availability
        {
            TenantId = tenantId,
            Weekday = request.Weekday,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            SlotDurationMinutes = request.SlotDurationMinutes,
            Modality = request.Modality ?? "online",
            Timezone = request.Timezone ?? "America/Sao_Paulo",
            IsActive = request.IsActive
        };

        dbContext.Availabilities.Add(availability);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(ToResult(availability));
    }

    public async Task<Result<IReadOnlyCollection<WeeklyAvailabilityResult>>> GetWeeklyAvailabilitiesAsync(int tenantId, CancellationToken cancellationToken)
    {
        var items = await dbContext.Availabilities
            .AsNoTracking()
            .Where(item => item.TenantId == tenantId)
            .OrderBy(item => item.Weekday)
            .ThenBy(item => item.StartTime)
            .Select(item => new WeeklyAvailabilityResult(item.Id, item.Weekday, item.StartTime, item.EndTime, item.SlotDurationMinutes, item.Modality, item.Timezone, item.IsActive))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyCollection<WeeklyAvailabilityResult>>(items);
    }

    public async Task<Result<bool>> UpdateWeeklyAvailabilityAsync(int availabilityId, WeeklyAvailabilityRequest request, int tenantId, CancellationToken cancellationToken)
    {
        var availability = await dbContext.Availabilities.FirstOrDefaultAsync(item => item.Id == availabilityId && item.TenantId == tenantId, cancellationToken);
        if (availability is null) return Result.Failure<bool>(Error.NotFound("Availability not found"));

        availability.Weekday = request.Weekday;
        availability.StartTime = request.StartTime;
        availability.EndTime = request.EndTime;
        availability.SlotDurationMinutes = request.SlotDurationMinutes;
        availability.Modality = request.Modality ?? availability.Modality;
        availability.Timezone = request.Timezone ?? availability.Timezone;
        availability.IsActive = request.IsActive;
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }

    public async Task<Result<bool>> DeleteWeeklyAvailabilityAsync(int availabilityId, int tenantId, CancellationToken cancellationToken)
    {
        var availability = await dbContext.Availabilities.FirstOrDefaultAsync(item => item.Id == availabilityId && item.TenantId == tenantId, cancellationToken);
        if (availability is null) return Result.Failure<bool>(Error.NotFound("Availability not found"));

        dbContext.Availabilities.Remove(availability);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }

    public async Task<Result<ScheduleBlockResult>> CreateScheduleBlockAsync(ScheduleBlockRequest request, int tenantId, int userId, CancellationToken cancellationToken)
    {
        var block = new ScheduleBlock { TenantId = tenantId, StartsAt = request.StartsAt, EndsAt = request.EndsAt, Reason = request.Reason, CreatedBy = userId };
        dbContext.ScheduleBlocks.Add(block);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(new ScheduleBlockResult(block.Id, block.StartsAt, block.EndsAt, block.Reason));
    }

    public async Task<Result<IReadOnlyCollection<ScheduleBlockResult>>> GetScheduleBlocksAsync(int tenantId, CancellationToken cancellationToken)
    {
        var items = await dbContext.ScheduleBlocks
            .AsNoTracking()
            .Where(item => item.TenantId == tenantId)
            .OrderBy(item => item.StartsAt)
            .Select(item => new ScheduleBlockResult(item.Id, item.StartsAt, item.EndsAt, item.Reason))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyCollection<ScheduleBlockResult>>(items);
    }

    public async Task<Result<bool>> DeleteScheduleBlockAsync(int blockId, int tenantId, CancellationToken cancellationToken)
    {
        var block = await dbContext.ScheduleBlocks.FirstOrDefaultAsync(item => item.Id == blockId && item.TenantId == tenantId, cancellationToken);
        if (block is null) return Result.Failure<bool>(Error.NotFound("Schedule block not found"));

        dbContext.ScheduleBlocks.Remove(block);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }

    public async Task<Result<IReadOnlyCollection<AvailableSlotResult>>> GetAvailableSlotsAsync(AvailableSlotsRequest request, int tenantId, CancellationToken cancellationToken)
    {
        var availabilities = await dbContext.Availabilities
            .Where(item => item.TenantId == tenantId && item.IsActive && (request.Modality == null || item.Modality == request.Modality))
            .ToListAsync(cancellationToken);
        var blocks = await dbContext.ScheduleBlocks.Where(item => item.TenantId == tenantId && item.StartsAt < request.To && item.EndsAt > request.From).ToListAsync(cancellationToken);
        var appointments = await dbContext.Appointments.Where(item => item.TenantId == tenantId && item.StartsAt < request.To && item.EndsAt > request.From && item.Status != "canceled").ToListAsync(cancellationToken);

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
        var appointment = await dbContext.Appointments.FirstOrDefaultAsync(item => item.Id == appointmentId && item.TenantId == tenantId, cancellationToken);
        if (appointment is null) return Result.Failure<bool>(Error.NotFound("Appointment not found"));

        appointment.Status = "canceled";
        appointment.CanceledAt = DateTime.UtcNow;
        appointment.CanceledBy = userId;
        appointment.CancelReason = request.Reason;
        appointment.LateCancel = request.LateCancel;
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }

    private static WeeklyAvailabilityResult ToResult(Availability availability) =>
        new(availability.Id, availability.Weekday, availability.StartTime, availability.EndTime, availability.SlotDurationMinutes, availability.Modality, availability.Timezone, availability.IsActive);
}
