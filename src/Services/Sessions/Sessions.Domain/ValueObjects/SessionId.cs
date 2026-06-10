using Core.Domain.Exceptions;
using Sessions.Domain.Errors;

namespace Sessions.Domain.ValueObjects;

public record SessionId
{
    public int Value { get; }

    public SessionId(int value)
    {
        if (value <= 0)
            throw new DomainException(SessionErrors.InvalidId);

        Value = value;
    }

    public override string ToString() => Value.ToString();
    public static implicit operator int(SessionId id) => id.Value;
}
