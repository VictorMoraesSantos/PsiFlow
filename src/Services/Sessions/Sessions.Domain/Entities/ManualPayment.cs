using Core.Domain.Aggregates;

namespace Sessions.Domain.Entities;

public class ManualPayment : BaseEntity<int>
{
    public int TenantId { get; set; }
    public int SessionId { get; set; }
    public string Status { get; set; } = ManualPaymentStatus.Pending;
    public int? AmountCents { get; set; }
    public string Currency { get; set; } = "BRL";
    public DateTime? ReceivedAt { get; set; }
    public int? MarkedBy { get; set; }
    public string? Notes { get; set; }
}
