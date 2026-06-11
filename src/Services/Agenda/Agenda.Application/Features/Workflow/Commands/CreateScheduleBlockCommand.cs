using Agenda.Application.Contracts;
using BuildingBlocks.CQRS.Requests.Commands;

namespace Agenda.Application.Features.Workflow;

public sealed record CreateScheduleBlockCommand(ScheduleBlockRequest Request, int TenantId, int UserId) : ICommand<ScheduleBlockResult>;
