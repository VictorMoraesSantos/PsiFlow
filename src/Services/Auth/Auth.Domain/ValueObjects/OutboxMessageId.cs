using Auth.Domain.Errors;
using Core.Domain.Exceptions;

namespace Auth.Domain.ValueObjects
{
    public record OutboxMessageId
    {
        public int Value { get; }
        public OutboxMessageId(int value)
        {
            if (value < 0)
                throw new DomainException(OutboxMessageErrors.InvalidId);
            Value = value;
        }
        public override string ToString() => Value.ToString();
        public static implicit operator int(OutboxMessageId id) => id.Value;
    }
}
