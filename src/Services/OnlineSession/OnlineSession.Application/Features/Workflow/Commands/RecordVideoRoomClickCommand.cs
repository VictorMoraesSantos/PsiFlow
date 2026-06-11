using BuildingBlocks.CQRS.Requests.Commands;

namespace OnlineSession.Application.Features.Workflow;

public sealed record RecordVideoRoomClickCommand(int SessionId, int TenantId, int? UserId, string? Role, string? IpAddress, string? UserAgent) : ICommand;
