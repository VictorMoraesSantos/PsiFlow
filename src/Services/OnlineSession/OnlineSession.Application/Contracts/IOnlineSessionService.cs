using BuildingBlocks.Results;

namespace OnlineSession.Application.Contracts;

public interface IOnlineSessionService
{
    Task<Result<object>> UpsertVideoRoomAsync(int sessionId, int tenantId, int userId, string? name, string provider, string url, string? instructions, CancellationToken ct);
    Task<Result<object>> GetVideoRoomAsync(int sessionId, int tenantId, CancellationToken ct);
    Task<Result> RecordClickAsync(int sessionId, int tenantId, int? userId, string? role, string? ipAddress, string? userAgent, CancellationToken ct);
    Task<Result<object>> GetClicksAsync(int sessionId, int tenantId, CancellationToken ct);
    Task<Result> UpsertDefaultSettingsAsync(int tenantId, string provider, string? url, CancellationToken ct);
    Task<Result<object>> GetDefaultSettingsAsync(int tenantId, CancellationToken ct);
}
