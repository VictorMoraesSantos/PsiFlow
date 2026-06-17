using Auth.Domain.Errors;
using Core.Domain.Exceptions;

namespace Auth.Domain.ValueObjects
{
    public record OutboxMessageId
    {
        public int Value { get; }

        public OutboxMessageId(int value)
        {
            Value = value;
        }

        public static OutboxMessageId Create(int value)
        {
            if (value <= 0)
                throw new DomainException(OutboxMessageErrors.InvalidId);
            var id = new OutboxMessageId(value);
            return id;
        }

        public override string ToString() => Value.ToString();
        public static implicit operator int(OutboxMessageId id) => id.Value;
    }
}
