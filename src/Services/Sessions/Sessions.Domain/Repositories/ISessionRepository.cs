using Sessions.Domain.Aggregates;
using Sessions.Domain.Filters;
using Core.Domain.Repositories;

namespace Sessions.Domain.Repositories
{
    public interface ISessionRepository : IRepository<Session, int, SessionQueryFilter> { }
}
