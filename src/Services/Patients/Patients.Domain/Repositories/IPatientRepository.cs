using Core.Domain.Repositories;
using Patients.Domain.Entities;
using Patients.Domain.Filters;

namespace Patients.Domain.Repositories
{
    public interface IPatientRepository : IRepository<Patient, int, PatientQueryFilter>
    {
        Task<Patient?> GetByIdAndTenantAsync(int id, int tenantId, CancellationToken cancellationToken = default);
    }
}
