using ClinicalRecords.Domain.Entities;
using ClinicalRecords.Domain.Repositories;
using Core.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using PsiFlow.ClinicalRecords.Infrastructure.Persistence.Data;

namespace ClinicalRecords.Infrastructure.Persistence.Repositories;

public sealed class AnamnesisVersionRepository(ClinicalRecordsDbContext dbContext) : Repository<AnamnesisVersion, int>(dbContext), IAnamnesisVersionRepository
{
    public async Task<int> CountByAnamnesisAsync(int anamnesisId, CancellationToken cancellationToken = default)
        => await dbContext.AnamnesisVersions.CountAsync(x => x.AnamnesisId == anamnesisId, cancellationToken);
}
