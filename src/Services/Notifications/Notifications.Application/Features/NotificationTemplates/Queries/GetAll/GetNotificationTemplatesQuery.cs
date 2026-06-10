using BuildingBlocks.CQRS.Requests.Queries;
using Notifications.Application.DTOs.NotificationTemplate;

namespace Notifications.Application.Features.NotificationTemplates.Queries.GetAll;

public sealed record GetNotificationTemplatesQuery : IQuery<IEnumerable<NotificationTemplateDTO?>>;
