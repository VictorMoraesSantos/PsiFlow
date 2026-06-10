using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Notifications.Application.Contracts;
using Notifications.Application.DTOs.NotificationTemplate;

namespace Notifications.Application.Features.NotificationTemplates.Queries.GetById;

public sealed class GetNotificationTemplateByIdQueryHandler(INotificationTemplateService service) : IQueryHandler<GetNotificationTemplateByIdQuery, NotificationTemplateDTO?>
{
    public Task<Result<NotificationTemplateDTO?>> Handle(GetNotificationTemplateByIdQuery query, CancellationToken cancellationToken) =>
        service.GetByIdAsync(query.Id, cancellationToken);
}
