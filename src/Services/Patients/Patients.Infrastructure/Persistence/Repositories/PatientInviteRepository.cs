using Microsoft.EntityFrameworkCore;
using Patients.Domain.Entities;
using Patients.Domain.Repositories;
using PsiFlow.Patients.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace Patients.Infrastructure.Persistence.Repositories;

public sealed class PatientInviteRepository(PatientsDbContext dbContext) : IPatientInviteRepository
{
    public async Task<PatientInvite?> GetById(int id, CancellationToken cancellationToken = default) =>
        await dbContext.PatientInvites.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<PatientInvite?>> GetAll(CancellationToken cancellationToken = default) =>
        await dbContext.PatientInvites.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<PatientInvite?>> Find(Expression<Func<PatientInvite, bool>> predicate, CancellationToken cancellationToken = default) =>
        await dbContext.PatientInvites.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task Create(PatientInvite entity, CancellationToken cancellationToken = default)
    {
        await dbContext.PatientInvites.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateRange(IEnumerable<PatientInvite> entities, CancellationToken cancellationToken = default)
    {
        await dbContext.PatientInvites.AddRangeAsync(entities, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(PatientInvite entity, CancellationToken cancellationToken = default)
    {
        dbContext.PatientInvites.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(PatientInvite entity, CancellationToken cancellationToken = default)
    {
        dbContext.PatientInvites.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> HasPendingForEmailAsync(int tenantId, string email, CancellationToken cancellationToken = default) =>
        await dbContext.PatientInvites.AnyAsync(x => x.TenantId == tenantId && x.Email == email && x.Status == "pending" && x.ExpiresAt > DateTime.UtcNow, cancellationToken);

    public async Task<PatientInvite?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default) =>
        await dbContext.PatientInvites.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

    public async Task<PatientInvite?> GetByIdAndTenantAsync(int id, int tenantId, CancellationToken cancellationToken = default) =>
        await dbContext.PatientInvites.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);
}
