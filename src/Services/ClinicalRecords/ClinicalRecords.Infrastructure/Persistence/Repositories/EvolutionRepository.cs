using ClinicalRecords.Domain.Entities;
using ClinicalRecords.Domain.Repositories;
using Core.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using PsiFlow.ClinicalRecords.Infrastructure.Persistence.Data;

namespace ClinicalRecords.Infrastructure.Persistence.Repositories;

public sealed class EvolutionRepository(ClinicalRecordsDbContext dbContext) : Repository<Evolution, int>(dbContext), IEvolutionRepository
{
    public async Task<Evolution?> GetBySessionAndTenantAsync(int sessionId, int tenantId, CancellationToken cancellationToken = default)
        => await dbContext.Evolutions.FirstOrDefaultAsync(x => x.SessionId == sessionId && x.TenantId == tenantId, cancellationToken);

    public async Task<int?> GetFirstRecordIdForTenantAsync(int tenantId, CancellationToken cancellationToken = default)
        => await dbContext.MedicalRecords.Where(x => x.TenantId == tenantId).Select(x => x.Id).FirstOrDefaultAsync(cancellationToken);
}
