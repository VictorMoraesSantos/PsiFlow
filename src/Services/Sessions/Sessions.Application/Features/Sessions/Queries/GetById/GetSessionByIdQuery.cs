using BuildingBlocks.CQRS.Requests.Queries;
using Sessions.Application.DTOs.Session;

namespace Sessions.Application.Features.Sessions.Queries.GetById;

public sealed record GetSessionByIdQuery(int Id) : IQuery<SessionDTO?>;
