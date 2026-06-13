using Auth.Domain.Errors;
using Core.Domain.Exceptions;

namespace Auth.Domain.ValueObjects
{
    public record RefreshTokenId
    {
        public int Value { get; }
        public RefreshTokenId(int value)
        {
            if (value <= 0)
                throw new DomainException(RefreshTokenErrors.InvalidId);
            Value = value;
        }

        public static implicit operator int(RefreshTokenId id) => id.Value;
    }
}
