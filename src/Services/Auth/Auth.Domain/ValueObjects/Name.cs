using Auth.Domain.Errors;
using Core.Domain.Exceptions;

namespace Auth.Domain.ValueObjects
{
    public class Name
    {
        public string FirstName { get; } = string.Empty;
        public string LastName { get; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";

        public Name(string firstName, string lastName)
        {
            Validate(firstName);
            Validate(lastName);
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
        }

        private void Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException(NameErrors.NullName);
        }
    }
}