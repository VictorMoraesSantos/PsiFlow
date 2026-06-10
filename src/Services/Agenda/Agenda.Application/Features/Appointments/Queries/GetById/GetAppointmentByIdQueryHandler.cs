using Agenda.Application.Contracts;
using Agenda.Application.DTOs.Appointment;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;

namespace Agenda.Application.Features.Appointments.Queries.GetById;

public sealed class GetAppointmentByIdQueryHandler(IAppointmentService service) : IQueryHandler<GetAppointmentByIdQuery, AppointmentDTO?>
{
    public Task<Result<AppointmentDTO?>> Handle(GetAppointmentByIdQuery query, CancellationToken cancellationToken) =>
        service.GetByIdAsync(query.Id, cancellationToken);
}
