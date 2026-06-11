using Agenda.Application.Contracts;
using BuildingBlocks.CQRS.Requests.Queries;

namespace Agenda.Application.Features.Workflow;

public sealed record GetWeeklyAvailabilitiesQuery(int TenantId) : IQuery<IReadOnlyCollection<WeeklyAvailabilityResult>>;
