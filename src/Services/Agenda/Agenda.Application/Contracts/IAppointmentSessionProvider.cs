using BuildingBlocks.Results;

namespace Agenda.Application.Contracts;

public interface IAppointmentSessionProvider
{
    Task<Result> CreateSessionForAppointmentAsync(AppointmentSessionRequest request, CancellationToken cancellationToken);
    Task<Result> CancelSessionForAppointmentAsync(int tenantId, int appointmentId, string? reason, CancellationToken cancellationToken);
}

public sealed record AppointmentSessionRequest(
    int TenantId,
    string Name,
    int AppointmentId,
    int PatientId,
    int PsychologistId,
    DateTime StartsAt,
    DateTime EndsAt,
    string Modality);
