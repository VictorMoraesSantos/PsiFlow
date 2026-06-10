using Core.Domain.Filters;
using Core.Infrastructure.Repositories;
using PsiFlow.Sessions.Infrastructure.Persistence;
using Sessions.Domain.Aggregates;
using Sessions.Domain.Filters;
using Sessions.Domain.Repositories;

namespace Sessions.Infrastructure.Persistence.Repositories;

public sealed class SessionRepository(SessionsDbContext dbContext) : Repository<Session, int, SessionQueryFilter>(dbContext), ISessionRepository
{
    protected override Specification<Session, int> CreateSpecification(SessionQueryFilter filter) => new SessionSpecification(filter);

    private sealed class SessionSpecification : Specification<Session, int>
    {
        public SessionSpecification(SessionQueryFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.TenantId.HasValue, x => x.TenantId == filter.TenantId!.Value);
            AddIf(!string.IsNullOrWhiteSpace(filter.Search), x => x.Name.Contains(filter.Search!) || x.Status.Contains(filter.Search!));
        }
    }
}
