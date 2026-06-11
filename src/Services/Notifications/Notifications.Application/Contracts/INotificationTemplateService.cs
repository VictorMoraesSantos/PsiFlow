using Core.Application.Interfaces;
using Notifications.Application.DTOs.NotificationTemplate;

namespace Notifications.Application.Contracts
{
    public interface INotificationTemplateService :
        IReadService<NotificationTemplateDTO, int, NotificationTemplateFilterDTO>,
        ICreateService<CreateNotificationTemplateDTO>,
        IUpdateService<UpdateNotificationTemplateDTO>,
        IDeleteService<int>
    {
    }
}
