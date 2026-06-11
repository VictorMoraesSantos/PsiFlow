using BuildingBlocks.CQRS.Requests.Commands;

namespace Agenda.Application.Features.Workflow;

public sealed record DeleteScheduleBlockCommand(int BlockId, int TenantId) : ICommand<bool>;
