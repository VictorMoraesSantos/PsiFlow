using Core.Domain.Exceptions;
using OnlineSession.Domain.Errors;

namespace OnlineSession.Domain.ValueObjects;

public record VideoRoomId
{
    public int Value { get; }

    public VideoRoomId(int value)
    {
        if (value <= 0)
            throw new DomainException(VideoRoomErrors.InvalidId);

        Value = value;
    }

    public override string ToString() => Value.ToString();
    public static implicit operator int(VideoRoomId id) => id.Value;
}
