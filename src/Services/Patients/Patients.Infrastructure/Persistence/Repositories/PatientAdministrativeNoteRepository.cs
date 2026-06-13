using Microsoft.EntityFrameworkCore;
using Patients.Domain.Entities;
using Patients.Domain.Repositories;
using PsiFlow.Patients.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace Patients.Infrastructure.Persistence.Repositories;

public sealed class PatientAdministrativeNoteRepository(PatientsDbContext dbContext) : IPatientAdministrativeNoteRepository
{
    public async Task<PatientAdministrativeNote?> GetById(int id, CancellationToken cancellationToken = default) =>
        await dbContext.PatientAdministrativeNotes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<PatientAdministrativeNote?>> GetAll(CancellationToken cancellationToken = default) =>
        await dbContext.PatientAdministrativeNotes.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<PatientAdministrativeNote?>> Find(Expression<Func<PatientAdministrativeNote, bool>> predicate, CancellationToken cancellationToken = default) =>
        await dbContext.PatientAdministrativeNotes.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task Create(PatientAdministrativeNote entity, CancellationToken cancellationToken = default)
    {
        await dbContext.PatientAdministrativeNotes.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateRange(IEnumerable<PatientAdministrativeNote> entities, CancellationToken cancellationToken = default)
    {
        await dbContext.PatientAdministrativeNotes.AddRangeAsync(entities, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(PatientAdministrativeNote entity, CancellationToken cancellationToken = default)
    {
        dbContext.PatientAdministrativeNotes.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(PatientAdministrativeNote entity, CancellationToken cancellationToken = default)
    {
        dbContext.PatientAdministrativeNotes.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
