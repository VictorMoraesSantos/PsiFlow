using Core.Domain.Repositories;
using Patients.Domain.Entities;

namespace Patients.Domain.Repositories
{
    public interface IPatientInviteRepository : IRepository<PatientInvite, int>
    {
        Task<bool> HasPendingForEmailAsync(int tenantId, string email, CancellationToken cancellationToken = default);
        Task<PatientInvite?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
        Task<PatientInvite?> GetByIdAndTenantAsync(int id, int tenantId, CancellationToken cancellationToken = default);
    }
}
