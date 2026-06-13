using Core.Domain.Repositories;
using Notifications.Domain.Entities;

namespace Notifications.Domain.Repositories
{
    public interface IScheduledNotificationRepository : IRepository<ScheduledNotification, int>
    {
        Task<IReadOnlyList<ScheduledNotification>> ClaimDueAsync(DateTime now, int maxBatch, int maxAttempts, CancellationToken cancellationToken);
        Task<IReadOnlyList<ScheduledNotification>> CancelByTypeAsync(string notificationType, string status, CancellationToken cancellationToken);
    }
}
