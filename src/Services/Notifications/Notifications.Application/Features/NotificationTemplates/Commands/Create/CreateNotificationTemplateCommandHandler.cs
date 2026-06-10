using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;
using Notifications.Application.Contracts;

namespace Notifications.Application.Features.NotificationTemplates.Commands.Create;

public sealed class CreateNotificationTemplateCommandHandler : ICommandHandler<CreateNotificationTemplateCommand, CreateNotificationTemplateResult>
{
    private readonly INotificationTemplateService _service;
    private readonly IValidator<CreateNotificationTemplateCommand> _validator;

    public CreateNotificationTemplateCommandHandler(INotificationTemplateService service, IValidator<CreateNotificationTemplateCommand> validator)
    {
        _service = service;
        _validator = validator;
    }

    public async Task<Result<CreateNotificationTemplateResult>> Handle(CreateNotificationTemplateCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure<CreateNotificationTemplateResult>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));

        var result = await _service.CreateAsync(command.NotificationTemplate, cancellationToken);
        return result.IsSuccess
            ? Result.Success(new CreateNotificationTemplateResult(result.Value))
            : Result.Failure<CreateNotificationTemplateResult>(result.Error!);
    }
}
