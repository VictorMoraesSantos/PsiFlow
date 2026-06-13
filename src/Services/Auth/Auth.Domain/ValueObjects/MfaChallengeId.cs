using Auth.Domain.Errors;
using Core.Domain.Exceptions;

namespace Auth.Domain.ValueObjects
{
    public record MfaChallengeId
    {
        public int Value { get; }

        public MfaChallengeId(int value)
        {
            Value = value;
        }

        public static MfaChallengeId Create(int value)
        {
            if (value <= 0)
                throw new DomainException(MfaChallengeErrors.InvalidId);
            return new MfaChallengeId(value);
        }

        public override string ToString() => Value.ToString();
        public static implicit operator int(MfaChallengeId id) => new MfaChallengeId(id);
    }
}
