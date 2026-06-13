using Core.Domain.Events;

namespace Auth.Domain.Events
{
    public sealed class PasswordResetRequestedDomainEvent : DomainEvent
    {
        public int UserId { get; }
        public int TenantId { get; }
        public string Email { get; }
        public string Token { get; }

        public PasswordResetRequestedDomainEvent(int userId, int tenantId, string email, string token)
        {
            UserId = userId;
            TenantId = tenantId;
            Email = email;
            Token = token;
        }
    }
}
