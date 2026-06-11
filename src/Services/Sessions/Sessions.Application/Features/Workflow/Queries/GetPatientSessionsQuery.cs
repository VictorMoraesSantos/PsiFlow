using BuildingBlocks.CQRS.Requests.Queries;
using Sessions.Application.Contracts;

namespace Sessions.Application.Features.Workflow;

public sealed record GetPatientSessionsQuery(int PatientId, int TenantId) : IQuery<IReadOnlyCollection<SessionResult>>;
