using ClinicalRecords.Domain.Entities;
using ClinicalRecords.Domain.Filters;
using ClinicalRecords.Domain.Filters.Specifications;
using ClinicalRecords.Domain.Repositories;
using Core.Domain.Filters;
using Core.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using PsiFlow.ClinicalRecords.Infrastructure.Persistence.Data;

namespace ClinicalRecords.Infrastructure.Persistence.Repositories;

public sealed class MedicalRecordRepository(ClinicalRecordsDbContext dbContext) : Repository<MedicalRecord, int, MedicalRecordQueryFilter>(dbContext), IMedicalRecordRepository
{
    protected override Specification<MedicalRecord, int> CreateSpecification(MedicalRecordQueryFilter filter) => new MedicalRecordSpecification(filter);

    public async Task<MedicalRecord?> GetByPatientAndTenantAsync(int patientId, int tenantId, CancellationToken cancellationToken = default)
        => await dbContext.MedicalRecords.AsNoTracking().FirstOrDefaultAsync(x => x.PatientId == patientId && x.TenantId == tenantId, cancellationToken);

    public async Task<MedicalRecord?> GetByIdAndTenantAsync(int id, int tenantId, CancellationToken cancellationToken = default)
        => await dbContext.MedicalRecords.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);
}
