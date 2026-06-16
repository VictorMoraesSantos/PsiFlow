using Patients.Domain.Entities;

namespace Patients.Domain.Repositories
{
    public interface IResponsibleLegalRepository
    {
        Task<ResponsibleLegal?> GetByPatientAsync(int patientId, int tenantId, CancellationToken cancellationToken = default);
        Task CreateAsync(ResponsibleLegal entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(ResponsibleLegal entity, CancellationToken cancellationToken = default);
    }
}
