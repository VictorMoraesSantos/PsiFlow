namespace Sessions.Domain.Entities;

public static class ManualPaymentStatus
{
    public const string Pending = "pending";
    public const string Received = "received";
    public const string Waived = "waived";
    public const string RefundedManual = "refunded_manual";

    public static readonly IReadOnlySet<string> All = new HashSet<string> { Pending, Received, Waived, RefundedManual };
}
