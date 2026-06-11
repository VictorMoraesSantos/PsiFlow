using Notifications.Domain.Entities;
using Notifications.Domain.Filters;
using Core.Domain.Repositories;

namespace Notifications.Domain.Repositories
{
    public interface INotificationTemplateRepository : IRepository<NotificationTemplate, int, NotificationTemplateQueryFilter> { }
}
