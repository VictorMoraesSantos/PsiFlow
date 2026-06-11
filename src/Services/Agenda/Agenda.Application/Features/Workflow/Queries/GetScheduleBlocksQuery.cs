using Agenda.Application.Contracts;
using BuildingBlocks.CQRS.Requests.Queries;

namespace Agenda.Application.Features.Workflow;

public sealed record GetScheduleBlocksQuery(int TenantId) : IQuery<IReadOnlyCollection<ScheduleBlockResult>>;
