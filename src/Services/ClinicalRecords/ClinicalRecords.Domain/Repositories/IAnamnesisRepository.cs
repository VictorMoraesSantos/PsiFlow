using ClinicalRecords.Domain.Entities;
using Core.Domain.Repositories;

namespace ClinicalRecords.Domain.Repositories
{
    public interface IAnamnesisRepository : IRepository<Anamnesis, int>
    {
        Task<Anamnesis?> GetByRecordAndTenantAsync(int recordId, int tenantId, CancellationToken cancellationToken = default);
    }
}
