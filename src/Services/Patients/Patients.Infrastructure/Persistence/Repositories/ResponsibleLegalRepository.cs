using Microsoft.EntityFrameworkCore;
using Patients.Domain.Entities;
using Patients.Domain.Repositories;
using PsiFlow.Patients.Infrastructure.Persistence.Data;

namespace Patients.Infrastructure.Persistence.Repositories;

public sealed class ResponsibleLegalRepository : IResponsibleLegalRepository
{
    private readonly PatientsDbContext _dbContext;

    public ResponsibleLegalRepository(PatientsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ResponsibleLegal?> GetByPatientAsync(int patientId, int tenantId, CancellationToken cancellationToken = default) =>
        _dbContext.ResponsibleLegals.FirstOrDefaultAsync(x => x.PatientId == patientId && x.TenantId == tenantId, cancellationToken);

    public async Task CreateAsync(ResponsibleLegal entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.ResponsibleLegals.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ResponsibleLegal entity, CancellationToken cancellationToken = default)
    {
        _dbContext.ResponsibleLegals.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
