using BuildingBlocks.CQRS.Requests.Commands;

namespace Agenda.Application.Features.Appointments.Commands.Delete;

public sealed record DeleteAppointmentCommand(int Id) : ICommand<bool>;
