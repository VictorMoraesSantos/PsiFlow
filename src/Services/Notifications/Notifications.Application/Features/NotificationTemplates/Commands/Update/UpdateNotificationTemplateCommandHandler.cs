using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;
using Notifications.Application.Contracts;

namespace Notifications.Application.Features.NotificationTemplates.Commands.Update;

public sealed class UpdateNotificationTemplateCommandHandler : ICommandHandler<UpdateNotificationTemplateCommand, bool>
{
    private readonly INotificationTemplateService _service;
    private readonly IValidator<UpdateNotificationTemplateCommand> _validator;

    public UpdateNotificationTemplateCommandHandler(INotificationTemplateService service, IValidator<UpdateNotificationTemplateCommand> validator)
    {
        _service = service;
        _validator = validator;
    }

    public async Task<Result<bool>> Handle(UpdateNotificationTemplateCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure<bool>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));

        return await _service.UpdateAsync(command.NotificationTemplate with { Id = command.Id }, cancellationToken);
    }
}
