using Core.Domain.Repositories;
using Sessions.Domain.Entities;

namespace Sessions.Domain.Repositories;

public interface ISessionStatusHistoryRepository : IRepository<SessionStatusHistory, int>
{
}
