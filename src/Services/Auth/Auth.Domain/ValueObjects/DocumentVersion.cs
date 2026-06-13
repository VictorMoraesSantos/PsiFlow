using Auth.Domain.Errors;
using Core.Domain.Exceptions;

namespace Auth.Domain.ValueObjects
{
    public sealed class DocumentVersion
    {
        public string Value { get; }

        private DocumentVersion(string value)
        {
            Value = value;
        }

        public static DocumentVersion Create(string? value, string field)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException(UserErrors.TermsNotAccepted);
            return new DocumentVersion(value.Trim());
        }

        public static DocumentVersion? CreateOptional(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return new DocumentVersion(value.Trim());
        }

        public override string ToString() => Value;

        public override bool Equals(object? obj) => obj is DocumentVersion v && v.Value == Value;
        public override int GetHashCode() => Value.GetHashCode();
    }
}
