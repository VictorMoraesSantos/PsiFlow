using Core.Domain.Exceptions;
using Notifications.Domain.Errors;

namespace Notifications.Domain.ValueObjects;

public record NotificationTemplateId
{
    public int Value { get; }

    public NotificationTemplateId(int value)
    {
        if (value <= 0)
            throw new DomainException(NotificationTemplateErrors.InvalidId);

        Value = value;
    }

    public override string ToString() => Value.ToString();
    public static implicit operator int(NotificationTemplateId id) => id.Value;
}
