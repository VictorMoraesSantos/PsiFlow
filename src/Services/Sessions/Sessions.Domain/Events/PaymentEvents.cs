using Core.Domain.Events;

namespace Sessions.Domain.Events;

public sealed class PaymentMarkedReceivedDomainEvent : DomainEvent
{
    public int PaymentId { get; }
    public int SessionId { get; }
    public int TenantId { get; }
    public int? AmountCents { get; }
    public string Currency { get; }
    public DateTime ReceivedAt { get; }
    public int MarkedBy { get; }

    public PaymentMarkedReceivedDomainEvent(int paymentId, int sessionId, int tenantId, int? amountCents, string currency, DateTime receivedAt, int markedBy)
    {
        PaymentId = paymentId;
        SessionId = sessionId;
        TenantId = tenantId;
        AmountCents = amountCents;
        Currency = currency;
        ReceivedAt = receivedAt;
        MarkedBy = markedBy;
    }
}
