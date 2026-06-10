using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Notifications.Application.Contracts;

namespace Notifications.Application.Features.NotificationTemplates.Commands.Delete;

public sealed class DeleteNotificationTemplateCommandHandler(INotificationTemplateService service) : ICommandHandler<DeleteNotificationTemplateCommand, bool>
{
    public Task<Result<bool>> Handle(DeleteNotificationTemplateCommand command, CancellationToken cancellationToken) =>
        service.DeleteAsync(command.Id, cancellationToken);
}
