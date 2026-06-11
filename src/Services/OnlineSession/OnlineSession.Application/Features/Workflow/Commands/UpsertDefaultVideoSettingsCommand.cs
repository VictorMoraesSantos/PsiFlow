using BuildingBlocks.CQRS.Requests.Commands;

namespace OnlineSession.Application.Features.Workflow;

public sealed record UpsertDefaultVideoSettingsCommand(int TenantId, string Provider, string? Url) : ICommand;
