using Auth.Application.DTOs.Auth;
using BuildingBlocks.CQRS.Requests.Queries;

namespace Auth.Application.Features.Auth.Queries.Me;

public sealed record MeQuery(int UserId) : IQuery<MeResponse>;
