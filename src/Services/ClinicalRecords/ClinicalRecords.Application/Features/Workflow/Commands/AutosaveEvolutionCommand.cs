using BuildingBlocks.CQRS.Requests.Commands;

namespace ClinicalRecords.Application.Features.Workflow;

public sealed record AutosaveEvolutionCommand(int SessionId, int TenantId, string? Ciphertext, string? Nonce, string? Tag) : ICommand;
