using BuildingBlocks.CQRS.Requests.Commands;

namespace ClinicalRecords.Application.Features.Workflow;

public sealed record PublishAnamnesisVersionCommand(int RecordId, int TenantId, int UserId, string? Reason) : ICommand<object>;
