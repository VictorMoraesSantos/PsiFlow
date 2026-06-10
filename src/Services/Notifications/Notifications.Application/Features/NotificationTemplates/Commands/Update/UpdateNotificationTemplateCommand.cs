using BuildingBlocks.CQRS.Requests.Commands;
using Notifications.Application.DTOs.NotificationTemplate;

namespace Notifications.Application.Features.NotificationTemplates.Commands.Update;

public sealed record UpdateNotificationTemplateCommand(int Id, UpdateNotificationTemplateDTO NotificationTemplate) : ICommand<bool>;
