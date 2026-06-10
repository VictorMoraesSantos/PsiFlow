using Core.Domain.Aggregates;

namespace Patients.Domain.Aggregates;

public class ResponsibleLegal : BaseEntity<int>
{
    public int TenantId { get; set; }
    public int PatientId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public string? DocumentEncrypted { get; set; }
}
