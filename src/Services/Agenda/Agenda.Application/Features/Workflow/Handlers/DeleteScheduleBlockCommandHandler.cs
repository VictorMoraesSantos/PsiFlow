using Agenda.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;

namespace Agenda.Application.Features.Workflow;

public sealed class DeleteScheduleBlockCommandHandler(IAgendaService service) : ICommandHandler<DeleteScheduleBlockCommand, bool>
{
    public Task<Result<bool>> Handle(DeleteScheduleBlockCommand command, CancellationToken cancellationToken) =>
        service.DeleteScheduleBlockAsync(command.BlockId, command.TenantId, cancellationToken);
}
