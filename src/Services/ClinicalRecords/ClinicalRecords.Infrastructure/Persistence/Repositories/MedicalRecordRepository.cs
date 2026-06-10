using ClinicalRecords.Domain.Aggregates;
using ClinicalRecords.Domain.Filters;
using ClinicalRecords.Domain.Repositories;
using Core.Domain.Filters;
using Core.Infrastructure.Repositories;
using PsiFlow.ClinicalRecords.Infrastructure.Persistence;

namespace ClinicalRecords.Infrastructure.Persistence.Repositories;

public sealed class MedicalRecordRepository(ClinicalRecordsDbContext dbContext) : Repository<MedicalRecord, int, MedicalRecordQueryFilter>(dbContext), IMedicalRecordRepository
{
    protected override Specification<MedicalRecord, int> CreateSpecification(MedicalRecordQueryFilter filter) => new MedicalRecordSpecification(filter);

    private sealed class MedicalRecordSpecification : Specification<MedicalRecord, int>
    {
        public MedicalRecordSpecification(MedicalRecordQueryFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.TenantId.HasValue, x => x.TenantId == filter.TenantId!.Value);
            AddIf(!string.IsNullOrWhiteSpace(filter.Search), x => x.Name.Contains(filter.Search!) || x.Status.Contains(filter.Search!));
        }
    }
}
