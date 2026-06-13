using Core.Domain.Events;

namespace Patients.Domain.Events
{
    public sealed class PatientInvitedDomainEvent : DomainEvent
    {
        public int InviteId { get; }
        public int TenantId { get; }
        public int? PatientId { get; }
        public string Email { get; }
        public string? Phone { get; }
        public string Token { get; }
        public DateTime ExpiresAt { get; }
        public PatientInvitedDomainEvent(int inviteId, int tenantId, int? patientId, string email, string? phone, string token)
        {
            InviteId = inviteId; TenantId = tenantId; PatientId = patientId; Email = email; Phone = phone; Token = token;
        }
    }

    public sealed class PatientInviteAcceptedDomainEvent : DomainEvent
    {
        public int InviteId { get; }
        public int TenantId { get; }
        public int? PatientId { get; }
        public string Email { get; }
        public int AcceptedByUserId { get; }
        public PatientInviteAcceptedDomainEvent(int inviteId, int tenantId, int? patientId, string email, int acceptedByUserId)
        {
            InviteId = inviteId; TenantId = tenantId; PatientId = patientId; Email = email; AcceptedByUserId = acceptedByUserId;
        }
    }
}
