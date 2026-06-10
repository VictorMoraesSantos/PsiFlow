using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Notifications.Application.Contracts;
using Notifications.Application.DTOs.NotificationTemplate;

namespace Notifications.Application.Features.NotificationTemplates.Queries.GetAll;

public sealed class GetNotificationTemplatesQueryHandler(INotificationTemplateService service) : IQueryHandler<GetNotificationTemplatesQuery, IEnumerable<NotificationTemplateDTO?>>
{
    public Task<Result<IEnumerable<NotificationTemplateDTO?>>> Handle(GetNotificationTemplatesQuery query, CancellationToken cancellationToken) =>
        service.GetAllAsync(cancellationToken);
}
