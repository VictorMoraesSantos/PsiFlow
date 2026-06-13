using ClinicalRecords.Domain.Entities;
using ClinicalRecords.Domain.Repositories;
using Core.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using PsiFlow.ClinicalRecords.Infrastructure.Persistence.Data;

namespace ClinicalRecords.Infrastructure.Persistence.Repositories;

public sealed class AnamnesisRepository(ClinicalRecordsDbContext dbContext) : Repository<Anamnesis, int>(dbContext), IAnamnesisRepository
{
    public async Task<Anamnesis?> GetByRecordAndTenantAsync(int recordId, int tenantId, CancellationToken cancellationToken = default)
        => await dbContext.Anamneses.FirstOrDefaultAsync(x => x.RecordId == recordId && x.TenantId == tenantId, cancellationToken);
}
