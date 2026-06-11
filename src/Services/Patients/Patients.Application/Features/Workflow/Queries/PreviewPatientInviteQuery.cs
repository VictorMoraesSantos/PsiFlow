using BuildingBlocks.CQRS.Requests.Queries;

namespace Patients.Application.Features.Workflow;

public sealed record PreviewPatientInviteQuery(string Token) : IQuery<object>;
