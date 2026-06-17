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

    public async Task<Result<MeResponse>> Handle(MeQuery query, CancellationToken cancellationToken)
    {
        var result = await _profile.GetMeAsync(query.UserId, cancellationToken);
        return result;
    }
}
