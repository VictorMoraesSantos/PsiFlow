using Agenda.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;

namespace Agenda.Application.Features.Appointments.Commands.Delete;

public sealed class DeleteAppointmentCommandHandler(IAppointmentService service) : ICommandHandler<DeleteAppointmentCommand, bool>
{
    public Task<Result<bool>> Handle(DeleteAppointmentCommand command, CancellationToken cancellationToken) =>
        service.DeleteAsync(command.Id, cancellationToken);
}
