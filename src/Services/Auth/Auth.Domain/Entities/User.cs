using Auth.Domain.ValueObjects;
using Core.Domain.Entities;
using Core.Domain.Events;
using Microsoft.AspNetCore.Identity;

namespace Auth.Domain.Entities
{
    public class User : IdentityUser<UserId>, IBaseEntity<UserId>
    {
        private readonly List<IDomainEvent> _domainEvents = new();
        public Name Name { get; private set; }
        public Contact Contact { get; private set; }
        public int RestaurantId { get; private set; }
        public int UnitId { get; set; }
        public DateTime? Birthday { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public bool IsActive { get; private set; } = true;
        public string? RefreshTokenHash { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        protected User()
        {
        }

        public User(Name name, Contact contact, int restaurantId, int unitId)
        {
            Name = name;
            Contact = contact;
            Email = contact.Email;
            UserName = contact.Email;
        }

        public DateTime CreatedAt { get; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; }
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void MarkAsUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
            MarkAsUpdated();
        }

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        public void UpdateProfile(Name name, Contact contact)
        {
            Name = name;
            Contact = contact;
            Email = contact.Email;
            UserName = contact.Email;
            NormalizedEmail = contact.Email.ToUpperInvariant();
            NormalizedUserName = contact.Email.ToUpperInvariant();
            MarkAsUpdated();
        }

        public void Deactivate()
        {
            IsActive = false;
            MarkAsUpdated();
        }

        public void Activate()
        {
            IsActive = true;
            MarkAsUpdated();
        }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            MarkAsUpdated();
        }
    }
}