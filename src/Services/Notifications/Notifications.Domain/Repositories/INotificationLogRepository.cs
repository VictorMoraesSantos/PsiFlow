using Core.Domain.Repositories;
using Notifications.Domain.Entities;

namespace Notifications.Domain.Repositories
{
    public interface INotificationLogRepository : IRepository<NotificationLog, int>
    {
        Task<NotificationLog?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<NotificationLog>> ListLatestAsync(int count, CancellationToken cancellationToken = default);
    }
}
