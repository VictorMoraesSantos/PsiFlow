using Core.Domain.Aggregates;

namespace Patients.Domain.Entities;

public class PatientAdministrativeNote : BaseEntity<int>
{
    public int TenantId { get; set; }
    public int PatientId { get; set; }
    public string Text { get; set; } = string.Empty;
    public int CreatedBy { get; set; }
}
