using Core.Domain.Repositories;
using Sessions.Domain.Entities;

namespace Sessions.Domain.Repositories;

public interface IReceiptRepository : IRepository<Receipt, int>
{
    Task<Receipt?> GetBySessionPaymentAsync(int sessionId, int paymentId, int tenantId, CancellationToken cancellationToken);
}
