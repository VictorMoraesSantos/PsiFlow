using Auth.Domain.Errors;
using Core.Domain.Exceptions;

namespace Auth.Domain.ValueObjects
{
    public record TenantId
    {
        public int Value { get; }
        public TenantId(int value)
        {
            if (value < 0)
                throw new DomainException(TenantErrors.InvalidId);
            Value = value;
        }

        public override string ToString() => Value.ToString();
        public static implicit operator int(TenantId id) => id.Value;
    }
}
