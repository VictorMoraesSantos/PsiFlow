using Agenda.Application.DTOs.Appointment;
using BuildingBlocks.CQRS.Requests.Queries;

namespace Agenda.Application.Features.Appointments.Queries.GetAll;

public sealed record GetAppointmentsQuery : IQuery<IEnumerable<AppointmentDTO?>>;
