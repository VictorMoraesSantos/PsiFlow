using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;

namespace Auth.Application.Features.Auth.Queries.Me;

public sealed class MeQueryHandler : IQueryHandler<MeQuery, MeResponse>
{
    private readonly IUserProfileService _profile;

    public MeQueryHandler(IUserProfileService profile)
    {
        _profile = profile;
    }

    public Task<Result<MeResponse>> Handle(MeQuery query, CancellationToken cancellationToken) =>
        _profile.GetMeAsync(query.UserId, cancellationToken);
}
