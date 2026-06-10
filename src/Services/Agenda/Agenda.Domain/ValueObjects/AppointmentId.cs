using Agenda.Domain.Errors;
using Core.Domain.Exceptions;

namespace Agenda.Domain.ValueObjects;

public record AppointmentId
{
    public int Value { get; }

    public AppointmentId(int value)
    {
        if (value <= 0)
            throw new DomainException(AppointmentErrors.InvalidId);

        Value = value;
    }

    public override string ToString() => Value.ToString();
    public static implicit operator int(AppointmentId id) => id.Value;
}
