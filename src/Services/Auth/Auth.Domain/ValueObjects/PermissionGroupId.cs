using Auth.Domain.Errors;
using Core.Domain.Exceptions;

namespace Auth.Domain.ValueObjects
{
    public record PermissionGroupId
    {
        public int Value { get; }

        public PermissionGroupId(int value)
        {
            Value = value;
        }

        public static PermissionGroupId Create(int value)
        {
            if (value <= 0)
                throw new DomainException(PermissionErrors.InvalidId);
            return new PermissionGroupId(value);
        }

        public override string ToString() => Value.ToString();
        public static implicit operator int(PermissionGroupId id) => id.Value;
    }
}
