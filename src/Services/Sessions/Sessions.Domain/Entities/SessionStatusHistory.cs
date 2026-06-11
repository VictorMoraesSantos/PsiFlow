using Core.Domain.Aggregates;

namespace Sessions.Domain.Entities;

public class SessionStatusHistory : BaseEntity<int>
{
    public int TenantId { get; set; }
    public int SessionId { get; set; }
    public string? FromStatus { get; set; }
    public string ToStatus { get; set; } = string.Empty;
    public int ChangedBy { get; set; }
    public string? Reason { get; set; }
    public int? CorrelationId { get; set; }
}
