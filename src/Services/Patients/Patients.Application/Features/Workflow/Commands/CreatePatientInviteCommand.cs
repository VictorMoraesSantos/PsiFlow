using BuildingBlocks.CQRS.Requests.Commands;

namespace Patients.Application.Features.Workflow;

public sealed record CreatePatientInviteCommand(string Email, string? Phone, int? PatientId, int TenantId, int UserId) : ICommand<object>;
