using ClinicalRecords.Domain.Entities;
using ClinicalRecords.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using PsiFlow.ClinicalRecords.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace ClinicalRecords.Infrastructure.Persistence.Repositories;

public sealed class ClinicalAuditLogRepository(ClinicalRecordsDbContext dbContext) : IClinicalAuditLogRepository
{
    public async Task<ClinicalAuditLog?> GetById(int id, CancellationToken cancellationToken = default) =>
        await dbContext.ClinicalAuditLogs.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<ClinicalAuditLog?>> GetAll(CancellationToken cancellationToken = default) =>
        await dbContext.ClinicalAuditLogs.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<ClinicalAuditLog?>> Find(Expression<Func<ClinicalAuditLog, bool>> predicate, CancellationToken cancellationToken = default) =>
        await dbContext.ClinicalAuditLogs.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task Create(ClinicalAuditLog entity, CancellationToken cancellationToken = default) =>
        await dbContext.ClinicalAuditLogs.AddAsync(entity, cancellationToken);

    public async Task CreateRange(IEnumerable<ClinicalAuditLog> entities, CancellationToken cancellationToken = default) =>
        await dbContext.ClinicalAuditLogs.AddRangeAsync(entities, cancellationToken);

    public Task Update(ClinicalAuditLog entity, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task Delete(ClinicalAuditLog entity, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public async Task<IReadOnlyCollection<ClinicalAuditLog>> ListByResourceAndTenantAsync(int resourceId, int tenantId, CancellationToken cancellationToken = default) =>
        await dbContext.ClinicalAuditLogs.AsNoTracking().Where(x => x.ResourceId == resourceId && x.TenantId == tenantId).ToListAsync(cancellationToken);
}
