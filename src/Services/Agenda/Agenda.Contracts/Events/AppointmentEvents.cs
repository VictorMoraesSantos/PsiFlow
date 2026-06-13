namespace Agenda.Contracts.Events;

public sealed record AppointmentScheduledIntegrationEvent(
    Guid EventId,
    DateTime OccurredAtUtc,
    int AppointmentId,
    int TenantId,
    int PatientId,
    int PsychologistId,
    DateTime StartsAt,
    DateTime EndsAt,
    string Modality);

public sealed record AppointmentCancelledIntegrationEvent(
    Guid EventId,
    DateTime OccurredAtUtc,
    int AppointmentId,
    int TenantId,
    int PatientId,
    int PsychologistId,
    DateTime CanceledAt,
    int CanceledBy,
    string? Reason,
    bool LateCancel);

public sealed record AppointmentRescheduledIntegrationEvent(
    Guid EventId,
    DateTime OccurredAtUtc,
    int PreviousAppointmentId,
    int NewAppointmentId,
    int TenantId,
    int PatientId,
    int PsychologistId);
