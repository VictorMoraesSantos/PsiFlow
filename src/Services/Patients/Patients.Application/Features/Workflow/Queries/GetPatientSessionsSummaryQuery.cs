using BuildingBlocks.CQRS.Requests.Queries;

namespace Patients.Application.Features.Workflow;

public sealed record GetPatientSessionsSummaryQuery(int PatientId, int TenantId) : IQuery<object>;
