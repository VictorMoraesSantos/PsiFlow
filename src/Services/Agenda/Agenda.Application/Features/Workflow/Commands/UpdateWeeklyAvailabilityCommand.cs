using Agenda.Application.Contracts;
using BuildingBlocks.CQRS.Requests.Commands;

namespace Agenda.Application.Features.Workflow;

public sealed record UpdateWeeklyAvailabilityCommand(int AvailabilityId, WeeklyAvailabilityRequest Request, int TenantId) : ICommand<bool>;
