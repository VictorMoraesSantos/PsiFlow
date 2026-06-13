using Core.Domain.Events;

namespace Sessions.Domain.Events;

public sealed class SessionStatusChangedDomainEvent : DomainEvent
{
    public int SessionId { get; }
    public int TenantId { get; }
    public int PatientId { get; }
    public int PsychologistId { get; }
    public string FromStatus { get; }
    public string ToStatus { get; }
    public int ChangedBy { get; }
    public string? Reason { get; }

    public SessionStatusChangedDomainEvent(int sessionId, int tenantId, int patientId, int psychologistId, string fromStatus, string toStatus, int changedBy, string? reason)
    {
        SessionId = sessionId;
        TenantId = tenantId;
        PatientId = patientId;
        PsychologistId = psychologistId;
        FromStatus = fromStatus;
        ToStatus = toStatus;
        ChangedBy = changedBy;
        Reason = reason;
    }
}

public sealed class ReceiptRequestedDomainEvent : DomainEvent
{
    public int ReceiptId { get; }
    public int SessionId { get; }
    public int PaymentId { get; }
    public int TenantId { get; }

    public ReceiptRequestedDomainEvent(int receiptId, int sessionId, int paymentId, int tenantId)
    {
        ReceiptId = receiptId;
        SessionId = sessionId;
        PaymentId = paymentId;
        TenantId = tenantId;
    }
}
