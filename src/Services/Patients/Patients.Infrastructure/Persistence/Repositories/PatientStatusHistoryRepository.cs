using Microsoft.EntityFrameworkCore;
using Patients.Domain.Entities;
using Patients.Domain.Repositories;
using PsiFlow.Patients.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace Patients.Infrastructure.Persistence.Repositories;

public sealed class PatientStatusHistoryRepository(PatientsDbContext dbContext) : IPatientStatusHistoryRepository
{
    public async Task<PatientStatusHistory?> GetById(int id, CancellationToken cancellationToken = default) =>
        await dbContext.PatientStatusHistories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<PatientStatusHistory?>> GetAll(CancellationToken cancellationToken = default) =>
        await dbContext.PatientStatusHistories.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<PatientStatusHistory?>> Find(Expression<Func<PatientStatusHistory, bool>> predicate, CancellationToken cancellationToken = default) =>
        await dbContext.PatientStatusHistories.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task Create(PatientStatusHistory entity, CancellationToken cancellationToken = default)
    {
        await dbContext.PatientStatusHistories.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateRange(IEnumerable<PatientStatusHistory> entities, CancellationToken cancellationToken = default)
    {
        await dbContext.PatientStatusHistories.AddRangeAsync(entities, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(PatientStatusHistory entity, CancellationToken cancellationToken = default)
    {
        dbContext.PatientStatusHistories.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(PatientStatusHistory entity, CancellationToken cancellationToken = default)
    {
        dbContext.PatientStatusHistories.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
