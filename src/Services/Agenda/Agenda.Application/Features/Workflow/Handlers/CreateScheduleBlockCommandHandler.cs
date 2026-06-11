using Agenda.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;

namespace Agenda.Application.Features.Workflow;

public sealed class CreateScheduleBlockCommandHandler(IAgendaService service, IValidator<CreateScheduleBlockCommand> validator) : ICommandHandler<CreateScheduleBlockCommand, ScheduleBlockResult>
{
    public async Task<Result<ScheduleBlockResult>> Handle(CreateScheduleBlockCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        return validation.IsValid
            ? await service.CreateScheduleBlockAsync(command.Request, command.TenantId, command.UserId, cancellationToken)
            : Result.Failure<ScheduleBlockResult>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));
    }
}
