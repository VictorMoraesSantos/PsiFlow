using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Sessions.Application.Contracts;
using Sessions.Application.DTOs.Session;

namespace Sessions.Application.Features.Sessions.Queries.GetById;

public sealed class GetSessionByIdQueryHandler(ISessionService service) : IQueryHandler<GetSessionByIdQuery, SessionDTO?>
{
    public Task<Result<SessionDTO?>> Handle(GetSessionByIdQuery query, CancellationToken cancellationToken) =>
        service.GetByIdAsync(query.Id, cancellationToken);
}
