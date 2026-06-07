using Auth.Domain.Errors;
using Core.Domain.Exceptions;

namespace Auth.Domain.ValueObjects
{
    public record PermissionGroupId
    {
        public int Value { get; }
        public PermissionGroupId(int value)
        {
            if (value <= 0)
                throw new DomainException(PermissionErrors.InvalidId);
            Value = value;
        }
        public override string ToString() => Value.ToString();
    }
}
