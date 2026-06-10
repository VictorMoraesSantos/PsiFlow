using BuildingBlocks.CQRS.Requests.Commands;

namespace Notifications.Application.Features.NotificationTemplates.Commands.Delete;

public sealed record DeleteNotificationTemplateCommand(int Id) : ICommand<bool>;
