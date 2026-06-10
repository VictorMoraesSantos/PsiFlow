using ClinicalRecords.Domain.Errors;
using Core.Domain.Exceptions;

namespace ClinicalRecords.Domain.ValueObjects;

public record MedicalRecordId
{
    public int Value { get; }

    public MedicalRecordId(int value)
    {
        if (value <= 0)
            throw new DomainException(MedicalRecordErrors.InvalidId);

        Value = value;
    }

    public override string ToString() => Value.ToString();
    public static implicit operator int(MedicalRecordId id) => id.Value;
}
