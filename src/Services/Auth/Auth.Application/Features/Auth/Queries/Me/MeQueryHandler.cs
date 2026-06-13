using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;

namespace Auth.Application.Features.Auth.Queries.Me;

public sealed class MeQueryHandler : IQueryHandler<MeQuery, MeResponse>
{
    private readonly IAuthService _service;

    public MeQueryHandler(IAuthService service)
    {
        _service = service;
    }

    public Task<Result<MeResponse>> Handle(MeQuery query, CancellationToken cancellationToken) =>
        _service.MeAsync(query.UserId, cancellationToken);
}
