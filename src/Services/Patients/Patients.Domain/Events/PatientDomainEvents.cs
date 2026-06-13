using Core.Domain.Events;

namespace Patients.Domain.Events
{
    public sealed class PatientCreatedDomainEvent : DomainEvent
    {
        public int PatientId { get; }
        public int TenantId { get; }
        public string FullName { get; }
        public string Email { get; }
        public PatientCreatedDomainEvent(int patientId, int tenantId, string fullName, string email)
        {
            PatientId = patientId; TenantId = tenantId; FullName = fullName; Email = email;
        }
    }

    public sealed class PatientDeactivatedDomainEvent : DomainEvent
    {
        public int PatientId { get; }
        public int TenantId { get; }
        public string FromStatus { get; }
        public string ToStatus { get; }
        public string? Reason { get; }
        public int ActorUserId { get; }
        public PatientDeactivatedDomainEvent(int patientId, int tenantId, string fromStatus, string toStatus, string? reason, int actorUserId)
        {
            PatientId = patientId; TenantId = tenantId; FromStatus = fromStatus; ToStatus = toStatus; Reason = reason; ActorUserId = actorUserId;
        }
    }
}
