using Auth.Domain.Errors;
using Core.Domain.Exceptions;

namespace Auth.Domain.ValueObjects
{
    public record ConsentId
    {
        public int Value { get; }
        public ConsentId(int value)
        {
            if (value < 0)
                throw new DomainException(ConsentErrors.InvalidId);
            Value = value;
        }

        public override string ToString() => Value.ToString();
        public static implicit operator int(ConsentId id) => id.Value;
    }
}
