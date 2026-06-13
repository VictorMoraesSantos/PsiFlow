using ClinicalRecords.Domain.Entities;
using ClinicalRecords.Domain.Repositories;
using Core.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using PsiFlow.ClinicalRecords.Infrastructure.Persistence.Data;

namespace ClinicalRecords.Infrastructure.Persistence.Repositories;

public sealed class EvolutionVersionRepository(ClinicalRecordsDbContext dbContext) : Repository<EvolutionVersion, int>(dbContext), IEvolutionVersionRepository
{
    public async Task<int> CountByEvolutionAsync(int evolutionId, CancellationToken cancellationToken = default)
        => await dbContext.EvolutionVersions.CountAsync(x => x.EvolutionId == evolutionId, cancellationToken);

    public async Task<IReadOnlyCollection<EvolutionVersion>> ListByEvolutionOrderedDescAsync(int evolutionId, CancellationToken cancellationToken = default)
        => await dbContext.EvolutionVersions.AsNoTracking().Where(x => x.EvolutionId == evolutionId).OrderByDescending(x => x.VersionNumber).ToListAsync(cancellationToken);
}
