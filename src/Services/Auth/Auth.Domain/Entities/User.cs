using Auth.Domain.Errors;
using Auth.Domain.Events;
using Auth.Domain.ValueObjects;
using Core.Domain.Aggregates;
using Core.Domain.Events;
using Core.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Auth.Domain.Entities
{
    public class User : IdentityUser<UserId>, IBaseEntity<UserId>
    {
        private readonly List<IDomainEvent> _domainEvents = new();
        public Name Name { get; private set; } = null!;
        public Contact Contact { get; private set; } = null!;
        public TenantId TenantId { get; private set; }
        public string Role { get; private set; } = UserRole.Patient;
        public string? Crp { get; private set; }
        public DateTime? Birthday { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public bool IsActive { get; private set; } = true;
        public ConsentId? CurrentConsentId { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; }
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected User() { }

        protected User(
            Name name,
            Contact contact,
            string role,
            TenantId? tenantId,
            string? crp)
        {
            Name = name;
            Contact = contact;
            Email = contact.Email;
            UserName = contact.Email;
            NormalizedEmail = contact.Email.ToUpperInvariant();
            NormalizedUserName = contact.Email.ToUpperInvariant();
            EmailConfirmed = false;
            Role = NormalizeRole(role);
            Crp = NormalizeCrp(crp, Role);
            TenantId = tenantId ?? new TenantId(0);
        }

        public static User Register(
            Name name,
            Contact contact,
            string role,
            TenantId? tenantId,
            string? crp)
        {
            var parsedRole = UserRole.Create(role);
            var parsedCrp = ValueObjects.Crp.Create(crp, parsedRole.RequiresCrp());
            var user = new User(
                name,
                contact,
                parsedRole.Value,
                tenantId,
                parsedCrp.Value);

            return user;
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

        public void UpdateBirthday(DateTime birthday)
        {
            if (birthday > DateTime.UtcNow)
                throw new DomainException(UserErrors.BirthDateInFuture);
            if (birthday < DateTime.UtcNow.AddYears(-150))
                throw new DomainException(UserErrors.InvalidBirthDate);
            Birthday = DateTime.SpecifyKind(birthday, DateTimeKind.Utc);
            MarkAsUpdated();
        }

        public Consent AcceptConsent(
            DocumentVersion termsVersion,
            DocumentVersion privacyVersion,
            string documentType,
            string? ipAddress,
            string? userAgent)
        {
            if (termsVersion is null) throw new DomainException(UserErrors.TermsNotAccepted);
            if (privacyVersion is null) throw new DomainException(UserErrors.TermsNotAccepted);

            var acceptedAt = DateTime.UtcNow;
            var consent = Consent.Accept(
                Id,
                new TenantId(TenantId.Value),
                termsVersion,
                privacyVersion,
                documentType,
                ipAddress,
                userAgent,
                acceptedAt);

            return consent;
        }

        public void LinkToConsent(ConsentId consentId)
        {
            if (consentId is null) throw new DomainException(UserErrors.TermsNotAccepted);
            CurrentConsentId = consentId;
            MarkAsUpdated();
        }

        public void Deactivate()
        {
            if (!IsActive) throw new DomainException(UserErrors.AlreadyInactive);
            IsActive = false;
            MarkAsUpdated();
        }

        public void Deactivate(string reason)
        {
            Deactivate();
            AddDomainEvent(new UserDeactivatedDomainEvent(Id, TenantId, reason ?? "unspecified"));
        }

        public void Activate()
        {
            if (IsActive) throw new DomainException(UserErrors.AlreadyActive);
            IsActive = true;
            MarkAsUpdated();
        }

        public void BeginLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            MarkAsUpdated();
        }

        public void ConfirmEmail()
        {
            EmailConfirmed = true;
            MarkAsUpdated();
        }

        public void RequestPasswordReset(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new DomainException(UserErrors.PasswordResetInvalid);
            AddDomainEvent(new PasswordResetRequestedDomainEvent(Id, TenantId, Email ?? string.Empty, token));
        }

        public void RequestEmailVerification(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new DomainException(UserErrors.InvalidCredentials);
            AddDomainEvent(new EmailVerificationRequestedDomainEvent(Id, TenantId, Email ?? string.Empty, token));
        }

        public void RegisterUser(Guid correlationId)
        {
            AddDomainEvent(new UserRegisteredDomainEvent(
                Id, TenantId, Email ?? string.Empty, Role, Name.FullName, correlationId));
        }

        public bool CanAuthenticate()
        {
            return IsActive && EmailConfirmed;
        }

        public UserRole GetRole() => UserRole.Create(Role);

        private static string NormalizeRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                throw new DomainException(UserErrors.RoleInvalid);
            return role.Trim().ToLowerInvariant();
        }

        private static string? NormalizeCrp(string? crp, string role)
        {
            if (string.IsNullOrWhiteSpace(crp))
            {
                if (role == UserRole.Psychologist)
                    throw new DomainException(UserErrors.CrpRequired);
                return null;
            }
            return crp.Trim();
        }
    }
}
