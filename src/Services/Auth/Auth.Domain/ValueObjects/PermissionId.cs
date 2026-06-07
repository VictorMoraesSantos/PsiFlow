using Auth.Domain.Errors;
using Core.Domain.Exceptions;

namespace Auth.Domain.ValueObjects
{
    public record PermissionId
    {
        public int Value { get; }
        public PermissionId(int value)
        {
            if (value <= 0)
                throw new DomainException(PermissionErrors.InvalidId);
            Value = value;
        }
        public override string ToString() => Value.ToString();
    }
}
