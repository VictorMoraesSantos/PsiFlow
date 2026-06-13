using ClinicalRecords.Domain.Entities;
using Core.Domain.Repositories;

namespace ClinicalRecords.Domain.Repositories
{
    public interface IClinicalAuditLogRepository : IRepository<ClinicalAuditLog, int>
    {
        Task<IReadOnlyCollection<ClinicalAuditLog>> ListByResourceAndTenantAsync(int resourceId, int tenantId, CancellationToken cancellationToken = default);
    }
}
