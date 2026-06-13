using Auth.Domain.Errors;
using Core.Domain.Exceptions;

namespace Auth.Domain.ValueObjects
{
    public record UserId
    {
        public int Value { get; }
        public UserId(int value)
        {
            if (value == 0)
                throw new DomainException(UserErrors.InvalidId);
            Value = value;
        }
        public override string ToString() => Value.ToString();
        public static implicit operator int(UserId id) => id.Value;
    }
}
