using Core.Domain.Repositories;
using Sessions.Domain.Entities;

namespace Sessions.Domain.Repositories;

public interface IManualPaymentRepository : IRepository<ManualPayment, int>
{
    Task<ManualPayment?> GetBySessionAndTenantAsync(int sessionId, int tenantId, CancellationToken cancellationToken);
}
