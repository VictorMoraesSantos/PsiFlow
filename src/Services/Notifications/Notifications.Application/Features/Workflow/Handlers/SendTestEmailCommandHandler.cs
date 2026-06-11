using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;
using Notifications.Application.Contracts;

namespace Notifications.Application.Features.Workflow;

public sealed class SendTestEmailCommandHandler(INotificationWorkflowService service, IValidator<SendTestEmailCommand> validator) : ICommandHandler<SendTestEmailCommand, object>
{
    public async Task<Result<object>> Handle(SendTestEmailCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        return validation.IsValid
            ? await service.SendTestEmailAsync(command.RecipientEmail, command.TemplateKey, command.TenantId, cancellationToken)
            : Result.Failure<object>(Error.Failure(string.Join("; ", validation.Errors.Select(x => x.ErrorMessage))));
    }
}
