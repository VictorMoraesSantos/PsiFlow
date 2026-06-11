using Core.Domain.Aggregates;

namespace ClinicalRecords.Domain.Entities;

public class ClinicalAuditLog : BaseEntity<int>
{
    public int TenantId { get; set; }
    public int ActorUserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public int ResourceId { get; set; }
    public int? PatientId { get; set; }
    public int? SessionId { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public int? CorrelationId { get; set; }
    public string Result { get; set; } = "success";
}
