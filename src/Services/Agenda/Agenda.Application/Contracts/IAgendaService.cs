using BuildingBlocks.Results;

namespace Agenda.Application.Contracts;

public interface IAgendaService
{
    Task<Result<WeeklyAvailabilityResult>> CreateWeeklyAvailabilityAsync(WeeklyAvailabilityRequest request, int tenantId, CancellationToken cancellationToken);
    Task<Result<IReadOnlyCollection<WeeklyAvailabilityResult>>> GetWeeklyAvailabilitiesAsync(int tenantId, CancellationToken cancellationToken);
    Task<Result<bool>> UpdateWeeklyAvailabilityAsync(int availabilityId, WeeklyAvailabilityRequest request, int tenantId, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteWeeklyAvailabilityAsync(int availabilityId, int tenantId, CancellationToken cancellationToken);
    Task<Result<ScheduleBlockResult>> CreateScheduleBlockAsync(ScheduleBlockRequest request, int tenantId, int userId, CancellationToken cancellationToken);
    Task<Result<IReadOnlyCollection<ScheduleBlockResult>>> GetScheduleBlocksAsync(int tenantId, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteScheduleBlockAsync(int blockId, int tenantId, CancellationToken cancellationToken);
    Task<Result<IReadOnlyCollection<AvailableSlotResult>>> GetAvailableSlotsAsync(AvailableSlotsRequest request, int tenantId, CancellationToken cancellationToken);
    Task<Result<bool>> CancelAppointmentAsync(int appointmentId, CancelAppointmentRequest request, int tenantId, int userId, CancellationToken cancellationToken);
}

public sealed record WeeklyAvailabilityRequest(int Weekday, TimeOnly StartTime, TimeOnly EndTime, int SlotDurationMinutes, string? Modality, string? Timezone, bool IsActive = true);
public sealed record WeeklyAvailabilityResult(int Id, int Weekday, TimeOnly StartTime, TimeOnly EndTime, int SlotDurationMinutes, string Modality, string Timezone, bool IsActive);
public sealed record ScheduleBlockRequest(DateTime StartsAt, DateTime EndsAt, string? Reason);
public sealed record ScheduleBlockResult(int Id, DateTime StartsAt, DateTime EndsAt, string? Reason);
public sealed record AvailableSlotsRequest(DateTime From, DateTime To, string? Modality);
public sealed record AvailableSlotResult(DateTime StartsAt, DateTime EndsAt, string Modality);
public sealed record CancelAppointmentRequest(string? Reason);
