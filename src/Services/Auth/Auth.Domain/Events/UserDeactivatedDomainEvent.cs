using Core.Domain.Events;

namespace Auth.Domain.Events
{
    public sealed class UserDeactivatedDomainEvent : DomainEvent
    {
        public int UserId { get; }
        public int TenantId { get; }
        public string Reason { get; }

        public UserDeactivatedDomainEvent(int userId, int tenantId, string reason)
        {
            UserId = userId;
            TenantId = tenantId;
            Reason = reason;
        }
    }
}
