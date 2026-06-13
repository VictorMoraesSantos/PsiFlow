using ClinicalRecords.Domain.Entities;
using ClinicalRecords.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using PsiFlow.ClinicalRecords.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace ClinicalRecords.Infrastructure.Persistence.Repositories;

public sealed class EvolutionRepository(ClinicalRecordsDbContext dbContext) : IEvolutionRepository
{
    public async Task<Evolution?> GetById(int id, CancellationToken cancellationToken = default) =>
        await dbContext.Evolutions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<Evolution?>> GetAll(CancellationToken cancellationToken = default) =>
        await dbContext.Evolutions.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<Evolution?>> Find(Expression<Func<Evolution, bool>> predicate, CancellationToken cancellationToken = default) =>
        await dbContext.Evolutions.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task Create(Evolution entity, CancellationToken cancellationToken = default) =>
        await dbContext.Evolutions.AddAsync(entity, cancellationToken);

    public async Task CreateRange(IEnumerable<Evolution> entities, CancellationToken cancellationToken = default) =>
        await dbContext.Evolutions.AddRangeAsync(entities, cancellationToken);

    public Task Update(Evolution entity, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task Delete(Evolution entity, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public async Task<Evolution?> GetBySessionAndTenantAsync(int sessionId, int tenantId, CancellationToken cancellationToken = default) =>
        await dbContext.Evolutions.FirstOrDefaultAsync(x => x.SessionId == sessionId && x.TenantId == tenantId, cancellationToken);

    public async Task<int?> GetFirstRecordIdForTenantAsync(int tenantId, CancellationToken cancellationToken = default) =>
        await dbContext.MedicalRecords.Where(x => x.TenantId == tenantId).Select(x => x.Id).FirstOrDefaultAsync(cancellationToken);
}
