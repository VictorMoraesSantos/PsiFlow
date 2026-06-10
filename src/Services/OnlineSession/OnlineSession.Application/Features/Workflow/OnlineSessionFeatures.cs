using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.CQRS.Requests.Commands;
using BuildingBlocks.CQRS.Requests.Queries;
using BuildingBlocks.Results;
using OnlineSession.Application.Contracts;

namespace OnlineSession.Application.Features.Workflow;

public sealed record VideoRoomBody(string? Name, string? Provider, string Url, string? Instructions);
public sealed record DefaultVideoSettingsBody(string? Provider, string? Url);
public sealed record UpsertVideoRoomCommand(int SessionId, int TenantId, int UserId, string? Name, string Provider, string Url, string? Instructions) : ICommand<object>;
public sealed record GetVideoRoomQuery(int SessionId, int TenantId) : IQuery<object>;
public sealed record RecordVideoRoomClickCommand(int SessionId, int TenantId, int? UserId, string? Role, string? IpAddress, string? UserAgent) : ICommand;
public sealed record GetVideoRoomClicksQuery(int SessionId, int TenantId) : IQuery<object>;
public sealed record UpsertDefaultVideoSettingsCommand(int TenantId, string Provider, string? Url) : ICommand;
public sealed record GetDefaultVideoSettingsQuery(int TenantId) : IQuery<object>;
public sealed class UpsertVideoRoomCommandHandler(IOnlineSessionService service) : ICommandHandler<UpsertVideoRoomCommand, object> { public Task<Result<object>> Handle(UpsertVideoRoomCommand c, CancellationToken ct) => service.UpsertVideoRoomAsync(c.SessionId, c.TenantId, c.UserId, c.Name, c.Provider, c.Url, c.Instructions, ct); }
public sealed class GetVideoRoomQueryHandler(IOnlineSessionService service) : IQueryHandler<GetVideoRoomQuery, object> { public Task<Result<object>> Handle(GetVideoRoomQuery q, CancellationToken ct) => service.GetVideoRoomAsync(q.SessionId, q.TenantId, ct); }
public sealed class RecordVideoRoomClickCommandHandler(IOnlineSessionService service) : ICommandHandler<RecordVideoRoomClickCommand> { public Task<Result> Handle(RecordVideoRoomClickCommand c, CancellationToken ct) => service.RecordClickAsync(c.SessionId, c.TenantId, c.UserId, c.Role, c.IpAddress, c.UserAgent, ct); }
public sealed class GetVideoRoomClicksQueryHandler(IOnlineSessionService service) : IQueryHandler<GetVideoRoomClicksQuery, object> { public Task<Result<object>> Handle(GetVideoRoomClicksQuery q, CancellationToken ct) => service.GetClicksAsync(q.SessionId, q.TenantId, ct); }
public sealed class UpsertDefaultVideoSettingsCommandHandler(IOnlineSessionService service) : ICommandHandler<UpsertDefaultVideoSettingsCommand> { public Task<Result> Handle(UpsertDefaultVideoSettingsCommand c, CancellationToken ct) => service.UpsertDefaultSettingsAsync(c.TenantId, c.Provider, c.Url, ct); }
public sealed class GetDefaultVideoSettingsQueryHandler(IOnlineSessionService service) : IQueryHandler<GetDefaultVideoSettingsQuery, object> { public Task<Result<object>> Handle(GetDefaultVideoSettingsQuery q, CancellationToken ct) => service.GetDefaultSettingsAsync(q.TenantId, ct); }
