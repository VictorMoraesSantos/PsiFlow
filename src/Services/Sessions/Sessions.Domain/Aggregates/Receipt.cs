using Core.Domain.Aggregates;

namespace Sessions.Domain.Aggregates;

public class Receipt : BaseEntity<int>
{
    public int TenantId { get; set; }
    public int SessionId { get; set; }
    public int PaymentId { get; set; }
    public string Status { get; set; } = "requested";
    public DateTime? SentAt { get; set; }
    public int? NotificationId { get; set; }
}
