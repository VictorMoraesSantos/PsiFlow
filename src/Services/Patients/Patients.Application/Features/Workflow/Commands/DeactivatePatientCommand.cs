using BuildingBlocks.CQRS.Requests.Commands;

namespace Patients.Application.Features.Workflow;

public sealed record DeactivatePatientCommand(int PatientId, string? Reason, int TenantId, int UserId) : ICommand;
