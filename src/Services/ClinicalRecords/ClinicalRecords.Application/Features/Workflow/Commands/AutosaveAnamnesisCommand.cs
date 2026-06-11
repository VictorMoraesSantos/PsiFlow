using BuildingBlocks.CQRS.Requests.Commands;

namespace ClinicalRecords.Application.Features.Workflow;

public sealed record AutosaveAnamnesisCommand(int RecordId, int TenantId, string? Ciphertext, string? Nonce, string? Tag) : ICommand;
