using Core.Domain.Filters;
using Core.Infrastructure.Repositories;
using PsiFlow.Sessions.Infrastructure.Persistence;
using Sessions.Domain.Aggregates;
using Sessions.Domain.Filters;
using Sessions.Domain.Filters.Specifications;
using Sessions.Domain.Repositories;

namespace Sessions.Infrastructure.Persistence.Repositories;

public sealed class SessionRepository(SessionsDbContext dbContext) : Repository<Session, int, SessionQueryFilter>(dbContext), ISessionRepository
{
    protected override Specification<Session, int> CreateSpecification(SessionQueryFilter filter) => new SessionSpecification(filter);
}
