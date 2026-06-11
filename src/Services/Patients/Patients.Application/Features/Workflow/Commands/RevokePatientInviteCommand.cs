using BuildingBlocks.CQRS.Requests.Commands;

namespace Patients.Application.Features.Workflow;

public sealed record RevokePatientInviteCommand(int InviteId, int TenantId) : ICommand;
