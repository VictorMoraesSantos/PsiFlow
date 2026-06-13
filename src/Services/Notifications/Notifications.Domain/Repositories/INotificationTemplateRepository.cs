using Core.Domain.Repositories;
using Notifications.Domain.Entities;
using Notifications.Domain.Filters;

namespace Notifications.Domain.Repositories
{
    public interface INotificationTemplateRepository : IRepository<NotificationTemplate, int, NotificationTemplateQueryFilter>
    {
        Task<bool> AnyByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
