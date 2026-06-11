using BuildingBlocks.CQRS.Requests.Commands;

namespace OnlineSession.Application.Features.Workflow;

public sealed record UpsertVideoRoomCommand(int SessionId, int TenantId, int UserId, string? Name, string Provider, string Url, string? Instructions) : ICommand<object>;
