using ClinicalRecords.Domain.Entities;
using Core.Domain.Repositories;

namespace ClinicalRecords.Domain.Repositories
{
    public interface IAnamnesisVersionRepository : IRepository<AnamnesisVersion, int>
    {
        Task<int> CountByAnamnesisAsync(int anamnesisId, CancellationToken cancellationToken = default);
    }
}
