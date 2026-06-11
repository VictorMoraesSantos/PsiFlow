using BuildingBlocks.CQRS.Requests.Commands;

namespace ClinicalRecords.Application.Features.Workflow;

public sealed record PublishEvolutionVersionCommand(int SessionId, int TenantId, int UserId, string? Reason) : ICommand<object>;
