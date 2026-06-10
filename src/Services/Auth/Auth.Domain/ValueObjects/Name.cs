using Auth.Domain.Errors;
using Core.Domain.Exceptions;

namespace Auth.Domain.ValueObjects
{
    public class Name
    {
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}".Trim();

        public Name() { }

        public Name(string firstName, string lastName)
        {
            Validate(firstName);
            Validate(lastName);
            FirstName = firstName.Trim();
            LastName = string.IsNullOrWhiteSpace(lastName) ? string.Empty : lastName.Trim();
        }

        public Name(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new DomainException(NameErrors.NullName);

            var parts = fullName.Trim().Split(' ', 2);
            FirstName = parts[0];
            LastName = parts.Length > 1 ? parts[1] : string.Empty;
        }

        private void Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException(NameErrors.NullName);
            if (value.Trim().Length < 2)
                throw new DomainException(NameErrors.TooShort);
            if (value.Length > 80)
                throw new DomainException(NameErrors.TooLong);
        }
    }
}
