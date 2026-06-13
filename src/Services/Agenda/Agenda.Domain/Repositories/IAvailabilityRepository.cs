using Agenda.Domain.Entities;
using Core.Domain.Repositories;

namespace Agenda.Domain.Repositories
{
    public interface IAvailabilityRepository : IRepository<Availability, int>
    {
        Task<bool> GetOverlappingAsync(int tenantId, int weekday, string modality, TimeOnly startTime, TimeOnly endTime, int? excludedId, CancellationToken cancellationToken = default);
        Task ReplaceTenantWeekAsync(int tenantId, IEnumerable<Availability> availabilities, CancellationToken cancellationToken = default);
    }
}
