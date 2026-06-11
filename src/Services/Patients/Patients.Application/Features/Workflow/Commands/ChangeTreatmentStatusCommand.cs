using BuildingBlocks.CQRS.Requests.Commands;

namespace Patients.Application.Features.Workflow;

public sealed record ChangeTreatmentStatusCommand(int PatientId, string TreatmentStatus, string? Reason, int TenantId, int UserId) : ICommand<object>;
