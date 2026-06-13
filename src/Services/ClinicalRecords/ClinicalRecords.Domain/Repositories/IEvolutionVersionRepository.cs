using ClinicalRecords.Domain.Entities;
using Core.Domain.Repositories;

namespace ClinicalRecords.Domain.Repositories
{
    public interface IEvolutionVersionRepository : IRepository<EvolutionVersion, int>
    {
        Task<int> CountByEvolutionAsync(int evolutionId, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<EvolutionVersion>> ListByEvolutionOrderedDescAsync(int evolutionId, CancellationToken cancellationToken = default);
    }
}
