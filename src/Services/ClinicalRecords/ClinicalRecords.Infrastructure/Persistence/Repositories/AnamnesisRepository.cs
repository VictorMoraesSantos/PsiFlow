using ClinicalRecords.Domain.Entities;
using ClinicalRecords.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using PsiFlow.ClinicalRecords.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace ClinicalRecords.Infrastructure.Persistence.Repositories;

public sealed class AnamnesisRepository(ClinicalRecordsDbContext dbContext) : IAnamnesisRepository
{
    public async Task<Anamnesis?> GetById(int id, CancellationToken cancellationToken = default) =>
        await dbContext.Anamneses.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<Anamnesis?>> GetAll(CancellationToken cancellationToken = default) =>
        await dbContext.Anamneses.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<Anamnesis?>> Find(Expression<Func<Anamnesis, bool>> predicate, CancellationToken cancellationToken = default) =>
        await dbContext.Anamneses.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task Create(Anamnesis entity, CancellationToken cancellationToken = default) =>
        await dbContext.Anamneses.AddAsync(entity, cancellationToken);

    public async Task CreateRange(IEnumerable<Anamnesis> entities, CancellationToken cancellationToken = default) =>
        await dbContext.Anamneses.AddRangeAsync(entities, cancellationToken);

    public Task Update(Anamnesis entity, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task Delete(Anamnesis entity, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public async Task<Anamnesis?> GetByRecordAndTenantAsync(int recordId, int tenantId, CancellationToken cancellationToken = default)
        => await dbContext.Anamneses.FirstOrDefaultAsync(x => x.RecordId == recordId && x.TenantId == tenantId, cancellationToken);
}
