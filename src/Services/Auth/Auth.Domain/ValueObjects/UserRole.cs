using Auth.Domain.Errors;
using Core.Domain.Exceptions;

namespace Auth.Domain.ValueObjects
{
    public sealed class UserRole
    {
        public const string Patient = "patient";
        public const string Psychologist = "psychologist";
        public const string SaasAdmin = "saas_admin";

        public string Value { get; }

        private UserRole(string value)
        {
            Value = value;
        }

        public static UserRole PatientRole() => new(Patient);
        public static UserRole PsychologistRole() => new(Psychologist);
        public static UserRole SaasAdminRole() => new(SaasAdmin);

        public static UserRole Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException(UserErrors.RoleInvalid);

            var normalized = value.Trim().ToLowerInvariant();
            return normalized switch
            {
                Patient => new UserRole(Patient),
                Psychologist => new UserRole(Psychologist),
                SaasAdmin => new UserRole(SaasAdmin),
                _ => throw new DomainException(UserErrors.RoleInvalid)
            };
        }

        public bool IsPsychologist() => Value == Psychologist;
        public bool IsSaasAdmin() => Value == SaasAdmin;
        public bool IsPatient() => Value == Patient;

        public bool AllowsMfa() => IsPsychologist() || IsSaasAdmin();
        public bool RequiresCrp() => IsPsychologist();

        public static bool TryParse(string? value, out UserRole? role)
        {
            role = null;
            if (string.IsNullOrWhiteSpace(value)) return false;
            var normalized = value.Trim().ToLowerInvariant();
            switch (normalized)
            {
                case Patient: role = PatientRole(); return true;
                case Psychologist: role = PsychologistRole(); return true;
                case SaasAdmin: role = SaasAdminRole(); return true;
                default: return false;
            }
        }

        public override string ToString() => Value;

        public override bool Equals(object? obj) => obj is UserRole r && r.Value == Value;
        public override int GetHashCode() => Value.GetHashCode();
    }
}
