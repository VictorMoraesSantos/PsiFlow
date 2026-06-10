using Auth.Domain.Errors;
using Core.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Auth.Domain.ValueObjects
{
    public class Contact
    {
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        private static readonly Regex PhoneRegex = new(@"^\+?[1-9]\d{7,14}$", RegexOptions.Compiled);

        public string Email { get; private set; } = string.Empty;
        public string? Phone { get; private set; }

        public Contact() { }

        public Contact(string email, string? phone = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException(ContactErrors.EmailRequired);
            if (!EmailRegex.IsMatch(email))
                throw new DomainException(ContactErrors.InvalidFormat);
            if (email.Length > 254)
                throw new DomainException(ContactErrors.EmailTooLong);
            if (phone is not null && !string.IsNullOrWhiteSpace(phone) && !PhoneRegex.IsMatch(phone.Replace(" ", "").Replace("-", "")))
                throw new DomainException(ContactErrors.InvalidPhone);

            Email = email.Trim().ToLowerInvariant();
            Phone = phone?.Trim();
        }
    }
}
