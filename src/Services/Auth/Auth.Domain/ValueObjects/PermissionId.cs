using Auth.Domain.Errors;
using Core.Domain.Exceptions;

namespace Auth.Domain.ValueObjects
{
    public record PermissionId
    {
        public int Value { get; }

        public PermissionId(int value)
        {
            Value = value;
        }

        public static PermissionId Create(int value)
        {
            if (value <= 0)
                throw new DomainException(PermissionErrors.InvalidId);
            var id = new PermissionId(value);
            return id;
        }

        public override string ToString() => Value.ToString();
        public static implicit operator int(PermissionId id) => id.Value;
    }
}
