using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using ClinicalRecords.Application.Contracts;

namespace ClinicalRecords.Application.Features.Workflow;

public sealed class GetClinicalRecordByPatientQueryHandler(IClinicalRecordWorkflowService service) : IQueryHandler<GetClinicalRecordByPatientQuery, object>
{
    public Task<Result<object>> Handle(GetClinicalRecordByPatientQuery query, CancellationToken cancellationToken) => service.GetRecordByPatientAsync(query.PatientId, query.TenantId, cancellationToken);
}
