using ClinicalRecords.Domain.Entities;
using Core.Domain.Repositories;

namespace ClinicalRecords.Domain.Repositories
{
    public interface IEvolutionRepository : IRepository<Evolution, int>
    {
        Task<Evolution?> GetBySessionAndTenantAsync(int sessionId, int tenantId, CancellationToken cancellationToken = default);
        Task<int?> GetFirstRecordIdForTenantAsync(int tenantId, CancellationToken cancellationToken = default);
    }
}
