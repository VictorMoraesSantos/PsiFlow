using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;
using Sessions.Application.Contracts;

namespace Sessions.Application.Features.Workflow;

public sealed class ChangeSessionStatusCommandHandler(ISessionWorkflowService service, IValidator<ChangeSessionStatusCommand> validator) : ICommandHandler<ChangeSessionStatusCommand, bool>
{
    public async Task<Result<bool>> Handle(ChangeSessionStatusCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        return validation.IsValid
            ? await service.ChangeStatusAsync(command.SessionId, command.Request, command.TenantId, command.UserId, cancellationToken)
            : Result.Failure<bool>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));
    }
}
