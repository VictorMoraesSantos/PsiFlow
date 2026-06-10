using Agenda.Application.DTOs.Appointment;
using BuildingBlocks.CQRS.Requests.Commands;

namespace Agenda.Application.Features.Appointments.Commands.Update;

public sealed record UpdateAppointmentCommand(int Id, UpdateAppointmentDTO Appointment) : ICommand<bool>;
