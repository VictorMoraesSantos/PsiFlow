using Core.Domain.Aggregates;

namespace Patients.Domain.Entities;

public class PatientStatusHistory : BaseEntity<int>
{
    public int TenantId { get; set; }
    public int PatientId { get; set; }
    public string FromStatus { get; set; } = string.Empty;
    public string ToStatus { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public int ChangedBy { get; set; }
    public string? CorrelationId { get; set; }
}
