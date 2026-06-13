using Core.Domain.Repositories;
using Notifications.Domain.Entities;

namespace Notifications.Domain.Repositories
{
    public interface IScheduledNotificationRepository : IRepository<ScheduledNotification, int> { }
}
