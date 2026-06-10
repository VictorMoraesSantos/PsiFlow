using BuildingBlocks.CQRS.Requests.Commands;
using Notifications.Application.DTOs.NotificationTemplate;

namespace Notifications.Application.Features.NotificationTemplates.Commands.Create;

public sealed record CreateNotificationTemplateCommand(CreateNotificationTemplateDTO NotificationTemplate) : ICommand<CreateNotificationTemplateResult>;
public sealed record CreateNotificationTemplateResult(int Id);
