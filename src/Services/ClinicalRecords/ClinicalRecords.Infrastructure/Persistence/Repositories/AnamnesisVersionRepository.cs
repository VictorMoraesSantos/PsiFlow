using ClinicalRecords.Domain.Entities;
using ClinicalRecords.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using PsiFlow.ClinicalRecords.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace ClinicalRecords.Infrastructure.Persistence.Repositories;

public sealed class AnamnesisVersionRepository(ClinicalRecordsDbContext dbContext) : IAnamnesisVersionRepository
{
    public async Task<AnamnesisVersion?> GetById(int id, CancellationToken cancellationToken = default) =>
        await dbContext.AnamnesisVersions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<AnamnesisVersion?>> GetAll(CancellationToken cancellationToken = default) =>
        await dbContext.AnamnesisVersions.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<AnamnesisVersion?>> Find(Expression<Func<AnamnesisVersion, bool>> predicate, CancellationToken cancellationToken = default) =>
        await dbContext.AnamnesisVersions.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task Create(AnamnesisVersion entity, CancellationToken cancellationToken = default) =>
        await dbContext.AnamnesisVersions.AddAsync(entity, cancellationToken);

    public async Task CreateRange(IEnumerable<AnamnesisVersion> entities, CancellationToken cancellationToken = default) =>
        await dbContext.AnamnesisVersions.AddRangeAsync(entities, cancellationToken);

    public Task Update(AnamnesisVersion entity, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task Delete(AnamnesisVersion entity, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public async Task<int> CountByAnamnesisAsync(int anamnesisId, CancellationToken cancellationToken = default) =>
        await dbContext.AnamnesisVersions.CountAsync(x => x.AnamnesisId == anamnesisId, cancellationToken);
}
