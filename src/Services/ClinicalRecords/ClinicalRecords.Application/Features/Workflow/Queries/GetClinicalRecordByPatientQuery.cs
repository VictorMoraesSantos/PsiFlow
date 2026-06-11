using BuildingBlocks.CQRS.Requests.Queries;

namespace ClinicalRecords.Application.Features.Workflow;

public sealed record GetClinicalRecordByPatientQuery(int PatientId, int TenantId) : IQuery<object>;
