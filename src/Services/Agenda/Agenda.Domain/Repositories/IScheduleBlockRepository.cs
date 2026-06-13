using Agenda.Domain.Entities;
using Core.Domain.Repositories;

namespace Agenda.Domain.Repositories
{
    public interface IScheduleBlockRepository : IRepository<ScheduleBlock, int>
    {
        Task<bool> ExistsForPeriodAsync(int tenantId, DateTime startsAt, DateTime endsAt, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<ScheduleBlock>> ListForPeriodAsync(int tenantId, DateTime from, DateTime to, CancellationToken cancellationToken = default);
    }
}
