using Core.Domain.Filters;
using Core.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using PsiFlow.Sessions.Infrastructure.Persistence.Data;
using Sessions.Domain.Entities;
using Sessions.Domain.Filters;
using Sessions.Domain.Filters.Specifications;
using Sessions.Domain.Repositories;

namespace Sessions.Infrastructure.Persistence.Repositories;

public sealed class SessionRepository(SessionsDbContext dbContext) : Repository<Session, int, SessionQueryFilter>(dbContext), ISessionRepository
{
    protected override Specification<Session, int> CreateSpecification(SessionQueryFilter filter) => new SessionSpecification(filter);

    public Task<Session?> GetByIdAndTenantAsync(int id, int tenantId, CancellationToken cancellationToken) =>
        dbContext.Sessions.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);

    public async Task<IReadOnlyCollection<Session>> ListByPatientOrderedAsync(int patientId, int tenantId, CancellationToken cancellationToken) =>
        await dbContext.Sessions.AsNoTracking()
            .Where(x => x.PatientId == patientId && x.TenantId == tenantId)
            .OrderBy(x => x.StartsAt)
            .ToListAsync(cancellationToken);
}
