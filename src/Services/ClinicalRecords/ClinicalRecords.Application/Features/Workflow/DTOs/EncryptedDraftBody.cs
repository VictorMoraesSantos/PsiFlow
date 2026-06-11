namespace ClinicalRecords.Application.Features.Workflow;

public sealed record EncryptedDraftBody(string? Ciphertext, string? Nonce, string? Tag);
