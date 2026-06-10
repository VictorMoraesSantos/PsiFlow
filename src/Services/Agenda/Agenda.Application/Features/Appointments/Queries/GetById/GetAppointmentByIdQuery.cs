using Agenda.Application.DTOs.Appointment;
using BuildingBlocks.CQRS.Requests.Queries;

namespace Agenda.Application.Features.Appointments.Queries.GetById;

public sealed record GetAppointmentByIdQuery(int Id) : IQuery<AppointmentDTO?>;
