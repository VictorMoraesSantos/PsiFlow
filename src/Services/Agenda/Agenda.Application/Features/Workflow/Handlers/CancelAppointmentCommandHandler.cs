using Agenda.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;

namespace Agenda.Application.Features.Workflow;

public sealed class CancelAppointmentCommandHandler(IAgendaService service) : ICommandHandler<CancelAppointmentCommand, bool>
{
    public Task<Result<bool>> Handle(CancelAppointmentCommand command, CancellationToken cancellationToken) =>
        service.CancelAppointmentAsync(command.AppointmentId, command.Request, command.TenantId, command.UserId, cancellationToken);
}
