using Core.Domain.Repositories;
using OnlineSession.Domain.Entities;

namespace OnlineSession.Domain.Repositories;

public interface IDefaultVideoProviderSettingsRepository : IRepository<DefaultVideoProviderSettings, int>
{
    Task<DefaultVideoProviderSettings?> GetByTenantAsync(int tenantId, CancellationToken cancellationToken = default);
}
