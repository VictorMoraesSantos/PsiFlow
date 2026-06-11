using BuildingBlocks.CQRS.Requests.Commands;

namespace Agenda.Application.Features.Workflow;

public sealed record DeleteWeeklyAvailabilityCommand(int AvailabilityId, int TenantId) : ICommand<bool>;
