using ClinicalRecords.Domain.Aggregates;
using ClinicalRecords.Domain.Filters;
using ClinicalRecords.Domain.Filters.Specifications;
using ClinicalRecords.Domain.Repositories;
using Core.Domain.Filters;
using Core.Infrastructure.Repositories;
using PsiFlow.ClinicalRecords.Infrastructure.Persistence;

namespace ClinicalRecords.Infrastructure.Persistence.Repositories;

public sealed class MedicalRecordRepository(ClinicalRecordsDbContext dbContext) : Repository<MedicalRecord, int, MedicalRecordQueryFilter>(dbContext), IMedicalRecordRepository
{
    protected override Specification<MedicalRecord, int> CreateSpecification(MedicalRecordQueryFilter filter) => new MedicalRecordSpecification(filter);
}
