using Auth.Domain.Errors;
using Core.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Auth.Domain.ValueObjects
{
    public sealed class Crp
    {
        private static readonly Regex CrpRegex = new(@"^\d{2}/\d{4,6}$", RegexOptions.Compiled);

        public string Value { get; }

        private Crp(string value)
        {
            Value = value;
        }

        public static Crp Create(string? value, bool required)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                if (required) throw new DomainException(UserErrors.CrpRequired);
                return new Crp(string.Empty);
            }

            var normalized = value.Trim();
            if (!CrpRegex.IsMatch(normalized))
                throw new DomainException(UserErrors.CrpInvalid);

            return new Crp(normalized);
        }

        public bool IsEmpty => string.IsNullOrEmpty(Value);

        public override string ToString() => Value;

        public override bool Equals(object? obj) => obj is Crp c && c.Value == Value;
        public override int GetHashCode() => Value.GetHashCode();
    }
}
