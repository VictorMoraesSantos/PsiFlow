using Core.Domain.Events;

namespace Auth.Domain.Events
{
    public sealed class UserRegisteredDomainEvent : DomainEvent
    {
        public int UserId { get; }
        public int TenantId { get; }
        public string Email { get; }
        public string Role { get; }
        public string FullName { get; }
        public Guid CorrelationId { get; }

        public UserRegisteredDomainEvent(int userId, int tenantId, string email, string role, string fullName, Guid correlationId)
        {
            UserId = userId;
            TenantId = tenantId;
            Email = email;
            Role = role;
            FullName = fullName;
            CorrelationId = correlationId;
        }
    }
}
