using Agenda.Application.DTOs.Appointment;
using BuildingBlocks.CQRS.Requests.Commands;

namespace Agenda.Application.Features.Appointments.Commands.Create;

public sealed record CreateAppointmentCommand(CreateAppointmentDTO Appointment) : ICommand<CreateAppointmentResult>;
public sealed record CreateAppointmentResult(int Id);
