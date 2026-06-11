using Agenda.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;

namespace Agenda.Application.Features.Workflow;

public sealed class GetWeeklyAvailabilitiesQueryHandler(IAgendaService service) : IQueryHandler<GetWeeklyAvailabilitiesQuery, IReadOnlyCollection<WeeklyAvailabilityResult>>
{
    public Task<Result<IReadOnlyCollection<WeeklyAvailabilityResult>>> Handle(GetWeeklyAvailabilitiesQuery query, CancellationToken cancellationToken) =>
        service.GetWeeklyAvailabilitiesAsync(query.TenantId, cancellationToken);
}
