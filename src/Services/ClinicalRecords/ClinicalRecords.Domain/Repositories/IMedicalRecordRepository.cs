using ClinicalRecords.Domain.Entities;
using ClinicalRecords.Domain.Filters;
using Core.Domain.Repositories;

namespace ClinicalRecords.Domain.Repositories
{
    public interface IMedicalRecordRepository : IRepository<MedicalRecord, int, MedicalRecordQueryFilter> { }
}
