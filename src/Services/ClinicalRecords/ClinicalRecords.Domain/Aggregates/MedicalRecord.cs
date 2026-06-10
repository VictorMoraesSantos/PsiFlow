using Core.Domain.Aggregates;

namespace ClinicalRecords.Domain.Aggregates;

public class MedicalRecord : BaseEntity<int>, IAggregateRoot
{
    public int TenantId { get; set; }
    public int PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "active";
}
