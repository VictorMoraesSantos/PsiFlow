using BuildingBlocks.CQRS.Requests.Commands;

namespace ClinicalRecords.Application.Features.Workflow;

public sealed record CreateClinicalRecordCommand(int PatientId, int TenantId, string? Name) : ICommand<object>;
