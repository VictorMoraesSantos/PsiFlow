using Core.Domain.Repositories;
using Sessions.Domain.Entities;
using Sessions.Domain.Filters;

namespace Sessions.Domain.Repositories;

public interface ISessionRepository : IRepository<Session, int, SessionQueryFilter>
{
    Task<Session?> GetByIdAndTenantAsync(int id, int tenantId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Session>> ListByPatientOrderedAsync(int patientId, int tenantId, CancellationToken cancellationToken);
}
