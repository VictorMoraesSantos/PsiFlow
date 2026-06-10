using BuildingBlocks.CQRS.Requests.Queries;
using Sessions.Application.DTOs.Session;

namespace Sessions.Application.Features.Sessions.Queries.GetAll;

public sealed record GetSessionsQuery : IQuery<IEnumerable<SessionDTO?>>;
