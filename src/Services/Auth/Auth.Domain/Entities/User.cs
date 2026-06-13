using Auth.Domain.Events;
using Auth.Domain.ValueObjects;
using Core.Domain.Aggregates;
using Core.Domain.Events;
using Microsoft.AspNetCore.Identity;

namespace Auth.Domain.Entities
{
    public class User : IdentityUser<UserId>, IBaseEntity<UserId>
    {
        private readonly List<IDomainEvent> _domainEvents = new();
        public Name Name { get; private set; } = null!;
        public Contact Contact { get; private set; } = null!;
        public TenantId TenantId { get; private set; }
        public string Role { get; private set; } = "patient";
        public string? Crp { get; private set; }
        public DateTime? Birthday { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public bool IsActive { get; private set; } = true;
        public string? ConsentTermsVersion { get; private set; }
        public string? ConsentPrivacyVersion { get; private set; }
        public DateTime? ConsentAcceptedAt { get; private set; }
        public bool IsMfaEnabled { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; }
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected User() { }

        public User(
            Name name,
            Contact contact,
            string role,
            TenantId? tenantId,
            string? crp,
            string termsVersion,
            string privacyVersion)
        {
            Name = name;
            Contact = contact;
            Email = contact.Email;
            UserName = contact.Email;
            NormalizedEmail = contact.Email.ToUpperInvariant();
            NormalizedUserName = contact.Email.ToUpperInvariant();
            EmailConfirmed = false;
            Role = role;
            Crp = crp;
            TenantId = tenantId ?? new TenantId(0);
            ConsentTermsVersion = termsVersion;
            ConsentPrivacyVersion = privacyVersion;
            ConsentAcceptedAt = DateTime.UtcNow;
        }

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

        public void AttachTenant(TenantId tenantId)
        {
            TenantId = tenantId;
            MarkAsUpdated();
        }

        public void RecordConsent(string termsVersion, string privacyVersion)
        {
            ConsentTermsVersion = termsVersion;
            ConsentPrivacyVersion = privacyVersion;
            ConsentAcceptedAt = DateTime.UtcNow;
            MarkAsUpdated();
            AddDomainEvent(new ConsentAcceptedDomainEvent(Id, TenantId, termsVersion, privacyVersion, DateTime.UtcNow));
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

        public void EnableMfa()
        {
            IsMfaEnabled = true;
            MarkAsUpdated();
        }
    }
}
