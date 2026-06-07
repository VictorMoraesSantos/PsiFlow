using Auth.Domain.Errors;
using Core.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Auth.Domain.ValueObjects
{
    public class Contact
    {
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public string Email { get; private set; }

        public Contact(string? email = null)
        {
            if (email != null && !EmailRegex.IsMatch(email))
                throw new DomainException(ContactErrors.InvalidFormat);

            Email = email;
        }
    }
}