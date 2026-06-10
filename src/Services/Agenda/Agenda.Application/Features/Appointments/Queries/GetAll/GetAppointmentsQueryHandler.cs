using Agenda.Application.Contracts;
using Agenda.Application.DTOs.Appointment;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;

namespace Agenda.Application.Features.Appointments.Queries.GetAll;

public sealed class GetAppointmentsQueryHandler(IAppointmentService service) : IQueryHandler<GetAppointmentsQuery, IEnumerable<AppointmentDTO?>>
{
    public Task<Result<IEnumerable<AppointmentDTO?>>> Handle(GetAppointmentsQuery query, CancellationToken cancellationToken) =>
        service.GetAllAsync(cancellationToken);
}
