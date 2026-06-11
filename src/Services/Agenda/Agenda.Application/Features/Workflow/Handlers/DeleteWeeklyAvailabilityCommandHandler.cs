using Agenda.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;

namespace Agenda.Application.Features.Workflow;

public sealed class DeleteWeeklyAvailabilityCommandHandler(IAgendaService service) : ICommandHandler<DeleteWeeklyAvailabilityCommand, bool>
{
    public Task<Result<bool>> Handle(DeleteWeeklyAvailabilityCommand command, CancellationToken cancellationToken) =>
        service.DeleteWeeklyAvailabilityAsync(command.AvailabilityId, command.TenantId, cancellationToken);
}
