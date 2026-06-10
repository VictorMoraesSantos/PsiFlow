using BuildingBlocks.CQRS.Requests.Queries;
using Notifications.Application.DTOs.NotificationTemplate;

namespace Notifications.Application.Features.NotificationTemplates.Queries.GetById;

public sealed record GetNotificationTemplateByIdQuery(int Id) : IQuery<NotificationTemplateDTO?>;
