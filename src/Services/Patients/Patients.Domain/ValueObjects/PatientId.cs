using Core.Domain.Exceptions;
using Patients.Domain.Errors;

namespace Patients.Domain.ValueObjects;

public record PatientId
{
    public int Value { get; }

    public PatientId(int value)
    {
        if (value <= 0)
            throw new DomainException(PatientErrors.InvalidId);

        Value = value;
    }

    public override string ToString() => Value.ToString();
    public static implicit operator int(PatientId id) => id.Value;
}
