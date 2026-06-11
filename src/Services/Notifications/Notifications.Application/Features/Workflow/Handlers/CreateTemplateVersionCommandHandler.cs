using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;
using Notifications.Application.Contracts;

namespace Notifications.Application.Features.Workflow;

public sealed class CreateTemplateVersionCommandHandler(INotificationWorkflowService service, IValidator<CreateTemplateVersionCommand> validator) : ICommandHandler<CreateTemplateVersionCommand, object>
{
    public async Task<Result<object>> Handle(CreateTemplateVersionCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        return validation.IsValid
            ? await service.CreateTemplateVersionAsync(command.TemplateId, command.Subject, command.BodyHtml, command.BodyText, command.UserId, cancellationToken)
            : Result.Failure<object>(Error.Failure(string.Join("; ", validation.Errors.Select(x => x.ErrorMessage))));
    }
}
