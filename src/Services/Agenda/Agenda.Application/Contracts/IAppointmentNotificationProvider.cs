using BuildingBlocks.Results;

namespace Agenda.Application.Contracts;

public interface IAppointmentNotificationProvider
{
    Task<Result> SendAppointmentScheduledAsync(AppointmentScheduledNotification notification, CancellationToken cancellationToken);
}

public sealed record AppointmentScheduledNotification(
    int TenantId,
    int AppointmentId,
    int PatientId,
    int PsychologistId,
    DateTime StartsAt,
    DateTime EndsAt,
    string Modality);
