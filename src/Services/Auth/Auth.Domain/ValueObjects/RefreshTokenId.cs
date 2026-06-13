using Auth.Domain.Errors;
using Core.Domain.Exceptions;

namespace Auth.Domain.ValueObjects
{
    public record RefreshTokenId
    {
        public int Value { get; }

        public RefreshTokenId(int value)
        {
            Value = value;
        }

        public static RefreshTokenId Create(int value)
        {
            if (value <= 0)
                throw new DomainException(RefreshTokenErrors.InvalidId);
            return new RefreshTokenId(value);
        }

        public static implicit operator int(RefreshTokenId id) => id.Value;
    }
}
