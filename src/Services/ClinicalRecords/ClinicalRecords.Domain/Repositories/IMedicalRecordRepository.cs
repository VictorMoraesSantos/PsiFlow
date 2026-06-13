using ClinicalRecords.Domain.Entities;
using ClinicalRecords.Domain.Filters;
using Core.Domain.Repositories;

namespace ClinicalRecords.Domain.Repositories
{
    public interface IMedicalRecordRepository : IRepository<MedicalRecord, int, MedicalRecordQueryFilter>
    {
        Task<MedicalRecord?> GetByPatientAndTenantAsync(int patientId, int tenantId, CancellationToken cancellationToken = default);
        Task<MedicalRecord?> GetByIdAndTenantAsync(int id, int tenantId, CancellationToken cancellationToken = default);
    }
}
