using Core.Domain.Repositories;
using Notifications.Domain.Entities;

namespace Notifications.Domain.Repositories
{
    public interface INotificationTemplateVersionRepository : IRepository<NotificationTemplateVersion, int>
    {
        Task<int> CountByTemplateAsync(int templateId, CancellationToken cancellationToken = default);
    }
}
