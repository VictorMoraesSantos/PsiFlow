using BuildingBlocks.CQRS.Requests.Commands;

namespace Notifications.Application.Features.Workflow;

public sealed record RetryNotificationCommand(int NotificationId) : ICommand;
