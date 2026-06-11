using System.Security.Cryptography;
using System.Text;
using BuildingBlocks.Results;
using Microsoft.EntityFrameworkCore;
using OnlineSession.Application.Contracts;
using OnlineSession.Domain.Entities;
using PsiFlow.OnlineSession.Infrastructure.Persistence.Data;

namespace OnlineSession.Infrastructure.Services;

public sealed class OnlineSessionService(OnlineSessionDbContext db) : IOnlineSessionService
{
    public async Task<Result<object>> UpsertVideoRoomAsync(int sessionId, int tenantId, int userId, string? name, string provider, string url, string? instructions, CancellationToken ct)
    {
        var room = await db.VideoRooms.FirstOrDefaultAsync(x => x.SessionId == sessionId && x.TenantId == tenantId, ct);
        if (room is null) { room = new VideoRoom { SessionId = sessionId, TenantId = tenantId, CreatedBy = userId }; db.VideoRooms.Add(room); }
        room.Name = string.IsNullOrWhiteSpace(name) ? $"Session {sessionId}" : name;
        room.Provider = provider;
        room.UrlEncrypted = url;
        room.UrlHash = Hash(url);
        room.Instructions = instructions;
        await db.SaveChangesAsync(ct);
        return Result.Success<object>(room);
    }
    public async Task<Result<object>> GetVideoRoomAsync(int sessionId, int tenantId, CancellationToken ct) { var room = await db.VideoRooms.AsNoTracking().FirstOrDefaultAsync(x => x.SessionId == sessionId && x.TenantId == tenantId, ct); return room is null ? Result.Failure<object>(Error.NotFound("Video room not found.")) : Result.Success<object>(room); }
    public async Task<Result> RecordClickAsync(int sessionId, int tenantId, int? userId, string? role, string? ipAddress, string? userAgent, CancellationToken ct) { var room = await db.VideoRooms.AsNoTracking().FirstOrDefaultAsync(x => x.SessionId == sessionId && x.TenantId == tenantId, ct); if (room is null) return Result.Failure(Error.NotFound("Video room not found.")); db.VideoRoomClicks.Add(new VideoRoomClick { TenantId = tenantId, SessionId = sessionId, VideoRoomId = room.Id, ClickedByUserId = userId, ClickedByRole = role, IpAddress = ipAddress, UserAgent = userAgent }); await db.SaveChangesAsync(ct); return Result.Success(); }
    public async Task<Result<object>> GetClicksAsync(int sessionId, int tenantId, CancellationToken ct) => Result.Success<object>(await db.VideoRoomClicks.AsNoTracking().Where(x => x.SessionId == sessionId && x.TenantId == tenantId).OrderByDescending(x => x.ClickedAt).ToListAsync(ct));
    public async Task<Result> UpsertDefaultSettingsAsync(int tenantId, string provider, string? url, CancellationToken ct) { var settings = await db.DefaultVideoProviderSettings.FirstOrDefaultAsync(x => x.TenantId == tenantId, ct); if (settings is null) { settings = new DefaultVideoProviderSettings { TenantId = tenantId }; db.DefaultVideoProviderSettings.Add(settings); } settings.DefaultProvider = provider; settings.DefaultUrlEncrypted = url; settings.DefaultUrlHash = string.IsNullOrWhiteSpace(url) ? null : Hash(url); await db.SaveChangesAsync(ct); return Result.Success(); }
    public async Task<Result<object>> GetDefaultSettingsAsync(int tenantId, CancellationToken ct) { var settings = await db.DefaultVideoProviderSettings.AsNoTracking().FirstOrDefaultAsync(x => x.TenantId == tenantId, ct); return settings is null ? Result.Success<object>(new { tenantId, defaultProvider = "external" }) : Result.Success<object>(settings); }
    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
