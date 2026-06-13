using ClinicalRecords.Domain.Entities;
using ClinicalRecords.Domain.Repositories;
using Core.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using PsiFlow.ClinicalRecords.Infrastructure.Persistence.Data;

namespace ClinicalRecords.Infrastructure.Persistence.Repositories;

public sealed class ClinicalAuditLogRepository(ClinicalRecordsDbContext dbContext) : Repository<ClinicalAuditLog, int>(dbContext), IClinicalAuditLogRepository
{
    public async Task<IReadOnlyCollection<ClinicalAuditLog>> ListByResourceAndTenantAsync(int resourceId, int tenantId, CancellationToken cancellationToken = default)
        => await dbContext.ClinicalAuditLogs.AsNoTracking().Where(x => x.ResourceId == resourceId && x.TenantId == tenantId).ToListAsync(cancellationToken);
}
