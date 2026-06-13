using ClinicalRecords.Domain.Entities;
using ClinicalRecords.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using PsiFlow.ClinicalRecords.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace ClinicalRecords.Infrastructure.Persistence.Repositories;

public sealed class EvolutionVersionRepository(ClinicalRecordsDbContext dbContext) : IEvolutionVersionRepository
{
    public async Task<EvolutionVersion?> GetById(int id, CancellationToken cancellationToken = default) =>
        await dbContext.EvolutionVersions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<EvolutionVersion?>> GetAll(CancellationToken cancellationToken = default) =>
        await dbContext.EvolutionVersions.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<EvolutionVersion?>> Find(Expression<Func<EvolutionVersion, bool>> predicate, CancellationToken cancellationToken = default) =>
        await dbContext.EvolutionVersions.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task Create(EvolutionVersion entity, CancellationToken cancellationToken = default) =>
        await dbContext.EvolutionVersions.AddAsync(entity, cancellationToken);

    public async Task CreateRange(IEnumerable<EvolutionVersion> entities, CancellationToken cancellationToken = default) =>
        await dbContext.EvolutionVersions.AddRangeAsync(entities, cancellationToken);

    public Task Update(EvolutionVersion entity, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task Delete(EvolutionVersion entity, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public async Task<int> CountByEvolutionAsync(int evolutionId, CancellationToken cancellationToken = default) =>
        await dbContext.EvolutionVersions.CountAsync(x => x.EvolutionId == evolutionId, cancellationToken);

    public async Task<IReadOnlyCollection<EvolutionVersion>> ListByEvolutionOrderedDescAsync(int evolutionId, CancellationToken cancellationToken = default) =>
        await dbContext.EvolutionVersions.AsNoTracking().Where(x => x.EvolutionId == evolutionId).OrderByDescending(x => x.VersionNumber).ToListAsync(cancellationToken);
}
