using Agenda.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;

namespace Agenda.Application.Features.Workflow;

public sealed class GetScheduleBlocksQueryHandler(IAgendaService service) : IQueryHandler<GetScheduleBlocksQuery, IReadOnlyCollection<ScheduleBlockResult>>
{
    public Task<Result<IReadOnlyCollection<ScheduleBlockResult>>> Handle(GetScheduleBlocksQuery query, CancellationToken cancellationToken) =>
        service.GetScheduleBlocksAsync(query.TenantId, cancellationToken);
}
