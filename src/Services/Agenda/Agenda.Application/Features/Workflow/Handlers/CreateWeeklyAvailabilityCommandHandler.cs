using Agenda.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;

namespace Agenda.Application.Features.Workflow;

public sealed class CreateWeeklyAvailabilityCommandHandler(IAgendaService service, IValidator<CreateWeeklyAvailabilityCommand> validator) : ICommandHandler<CreateWeeklyAvailabilityCommand, WeeklyAvailabilityResult>
{
    public async Task<Result<WeeklyAvailabilityResult>> Handle(CreateWeeklyAvailabilityCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        return validation.IsValid
            ? await service.CreateWeeklyAvailabilityAsync(command.Request, command.TenantId, cancellationToken)
            : Result.Failure<WeeklyAvailabilityResult>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));
    }
}
