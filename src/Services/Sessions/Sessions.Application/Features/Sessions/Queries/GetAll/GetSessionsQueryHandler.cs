using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Sessions.Application.Contracts;
using Sessions.Application.DTOs.Session;

namespace Sessions.Application.Features.Sessions.Queries.GetAll;

public sealed class GetSessionsQueryHandler(ISessionService service) : IQueryHandler<GetSessionsQuery, IEnumerable<SessionDTO?>>
{
    public Task<Result<IEnumerable<SessionDTO?>>> Handle(GetSessionsQuery query, CancellationToken cancellationToken) =>
        service.GetAllAsync(cancellationToken);
}
