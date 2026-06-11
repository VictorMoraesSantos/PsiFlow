using BuildingBlocks.CQRS.Requests.Queries;

namespace ClinicalRecords.Application.Features.Workflow;

public sealed record GetClinicalRecordQuery(int RecordId, int TenantId) : IQuery<object>;
