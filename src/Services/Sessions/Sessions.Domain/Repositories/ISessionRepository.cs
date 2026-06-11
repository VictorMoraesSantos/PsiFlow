using Core.Domain.Repositories;
using Sessions.Domain.Entities;
using Sessions.Domain.Filters;

namespace Sessions.Domain.Repositories
{
    public interface ISessionRepository : IRepository<Session, int, SessionQueryFilter> { }
}
