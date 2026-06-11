using Agenda.Application.Contracts;
using BuildingBlocks.CQRS.Requests.Commands;

namespace Agenda.Application.Features.Workflow;

public sealed record CreateWeeklyAvailabilityCommand(WeeklyAvailabilityRequest Request, int TenantId) : ICommand<WeeklyAvailabilityResult>;
