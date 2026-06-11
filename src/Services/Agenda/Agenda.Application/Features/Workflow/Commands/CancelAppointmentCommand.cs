using Agenda.Application.Contracts;
using BuildingBlocks.CQRS.Requests.Commands;

namespace Agenda.Application.Features.Workflow;

public sealed record CancelAppointmentCommand(int AppointmentId, CancelAppointmentRequest Request, int TenantId, int UserId) : ICommand<bool>;
