using Auth.Domain.Errors;
using Core.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Auth.Domain.ValueObjects
{
    public sealed class PasswordPolicy
    {
        public const int MinLength = 10;
        public const int MaxLength = 128;

        private static readonly Regex StrongPasswordRegex = new(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{10,128}$",
            RegexOptions.Compiled);

        public string Value { get; }

        private PasswordPolicy(string value)
        {
            Value = value;
        }

        public static PasswordPolicy Create(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException(UserErrors.PasswordTooWeak);
            if (!StrongPasswordRegex.IsMatch(value))
                throw new DomainException(UserErrors.PasswordTooWeak);
            var policy = new PasswordPolicy(value);
            return policy;
        }

        public static void EnsureMatch(PasswordPolicy current, PasswordPolicy confirmation)
        {
            if (!string.Equals(current.Value, confirmation.Value, StringComparison.Ordinal))
                throw new DomainException(UserErrors.PasswordsDoNotMatch);
        }

        public override string ToString() => Value;

        public override bool Equals(object? obj) => obj is PasswordPolicy p && p.Value == Value;
        public override int GetHashCode() => Value.GetHashCode();
    }
}
