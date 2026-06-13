using Core.Domain.Events;

namespace Agenda.Domain.Events;

public sealed class ScheduleBlockAffectsAppointmentsDomainEvent : DomainEvent
{
    public int ScheduleBlockId { get; }
    public int TenantId { get; }
    public DateTime StartsAt { get; }
    public DateTime EndsAt { get; }
    public IReadOnlyCollection<int> AppointmentIds { get; }

    public ScheduleBlockAffectsAppointmentsDomainEvent(int scheduleBlockId, int tenantId, DateTime startsAt, DateTime endsAt, IReadOnlyCollection<int> appointmentIds)
    {
        ScheduleBlockId = scheduleBlockId;
        TenantId = tenantId;
        StartsAt = startsAt;
        EndsAt = endsAt;
        AppointmentIds = appointmentIds;
    }
}
