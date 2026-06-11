using Agenda.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;

namespace Agenda.Application.Features.Workflow;

public sealed class UpdateWeeklyAvailabilityCommandHandler(IAgendaService service, IValidator<UpdateWeeklyAvailabilityCommand> validator) : ICommandHandler<UpdateWeeklyAvailabilityCommand, bool>
{
    public async Task<Result<bool>> Handle(UpdateWeeklyAvailabilityCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        return validation.IsValid
            ? await service.UpdateWeeklyAvailabilityAsync(command.AvailabilityId, command.Request, command.TenantId, cancellationToken)
            : Result.Failure<bool>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));
    }
}
