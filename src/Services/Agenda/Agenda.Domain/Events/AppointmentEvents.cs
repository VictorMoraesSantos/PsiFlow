using Core.Domain.Events;

namespace Agenda.Domain.Events;

public sealed class AppointmentScheduledDomainEvent : DomainEvent
{
    public int AppointmentId { get; }
    public int TenantId { get; }
    public int PatientId { get; }
    public int PsychologistId { get; }
    public DateTime StartsAt { get; }
    public DateTime EndsAt { get; }
    public string Modality { get; }

    public AppointmentScheduledDomainEvent(int appointmentId, int tenantId, int patientId, int psychologistId, DateTime startsAt, DateTime endsAt, string modality)
    {
        AppointmentId = appointmentId;
        TenantId = tenantId;
        PatientId = patientId;
        PsychologistId = psychologistId;
        StartsAt = startsAt;
        EndsAt = endsAt;
        Modality = modality;
    }
}

public sealed class AppointmentCancelledDomainEvent : DomainEvent
{
    public int AppointmentId { get; }
    public int TenantId { get; }
    public int PatientId { get; }
    public int PsychologistId { get; }
    public DateTime CanceledAt { get; }
    public int CanceledBy { get; }
    public string? Reason { get; }
    public bool LateCancel { get; }

    public AppointmentCancelledDomainEvent(int appointmentId, int tenantId, int patientId, int psychologistId, DateTime canceledAt, int canceledBy, string? reason, bool lateCancel)
    {
        AppointmentId = appointmentId;
        TenantId = tenantId;
        PatientId = patientId;
        PsychologistId = psychologistId;
        CanceledAt = canceledAt;
        CanceledBy = canceledBy;
        Reason = reason;
        LateCancel = lateCancel;
    }
}

public sealed class AppointmentRescheduledDomainEvent : DomainEvent
{
    public int PreviousAppointmentId { get; }
    public int NewAppointmentId { get; }
    public int TenantId { get; }
    public int PatientId { get; }
    public int PsychologistId { get; }

    public AppointmentRescheduledDomainEvent(int previousAppointmentId, int newAppointmentId, int tenantId, int patientId, int psychologistId)
    {
        PreviousAppointmentId = previousAppointmentId;
        NewAppointmentId = newAppointmentId;
        TenantId = tenantId;
        PatientId = patientId;
        PsychologistId = psychologistId;
    }
}
