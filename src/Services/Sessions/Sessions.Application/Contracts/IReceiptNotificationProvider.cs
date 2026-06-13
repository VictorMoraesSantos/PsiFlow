using BuildingBlocks.Results;

namespace Sessions.Application.Contracts;

public interface IReceiptNotificationProvider
{
    Task<Result<int?>> SendReceiptAsync(ReceiptNotificationRequest request, CancellationToken cancellationToken);
}

public sealed record ReceiptNotificationRequest(int TenantId, int SessionId, int PaymentId, int? AmountCents, string Currency, DateTime? ReceivedAt);
