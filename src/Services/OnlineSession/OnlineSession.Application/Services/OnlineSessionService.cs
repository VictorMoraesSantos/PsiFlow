using BuildingBlocks.Results;
using OnlineSession.Application.Contracts;
using OnlineSession.Domain.Entities;
using OnlineSession.Domain.Errors;
using OnlineSession.Domain.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace OnlineSession.Application.Services;

public sealed class OnlineSessionService(
    IVideoRoomRepository videoRooms,
    IVideoRoomClickRepository videoRoomClicks,
    IDefaultVideoProviderSettingsRepository defaultSettings) : IOnlineSessionService
{
    public async Task<Result<object>> UpsertVideoRoomAsync(int sessionId, int tenantId, int userId, string? name, string provider, string url, string? instructions, CancellationToken ct)
    {
        var urlValidation = ValidateVideoUrl(provider, url);
        if (!urlValidation.IsSuccess) return Result.Failure<object>(urlValidation.Error!);

        var room = await videoRooms.GetBySessionAndTenantAsync(sessionId, tenantId, ct);
        if (room is null)
        {
            room = new VideoRoom { SessionId = sessionId, TenantId = tenantId, CreatedBy = userId, Name = string.IsNullOrWhiteSpace(name) ? $"Session {sessionId}" : name, Provider = provider, UrlEncrypted = url, UrlHash = Hash(url), Instructions = instructions };
            await videoRooms.Create(room, ct);
        }
        else
        {
            room.Name = string.IsNullOrWhiteSpace(name) ? $"Session {sessionId}" : name;
            room.Provider = provider;
            room.UrlEncrypted = url;
            room.UrlHash = Hash(url);
            room.Instructions = instructions;
            await videoRooms.Update(room, ct);
        }
        return Result.Success<object>(room);
    }

    public async Task<Result<object>> GetVideoRoomAsync(int sessionId, int tenantId, CancellationToken ct)
    {
        var room = await videoRooms.GetBySessionAndTenantNoTrackAsync(sessionId, tenantId, ct);
        if (room is null) return Result.Failure<object>(Error.NotFound("Video room not found."));
        if (string.IsNullOrWhiteSpace(room.UrlEncrypted))
        {
            var settings = await defaultSettings.GetByTenantAsync(tenantId, ct);
            if (settings is not null && !string.IsNullOrWhiteSpace(settings.DefaultUrlEncrypted))
            {
                room.Provider = settings.DefaultProvider;
                room.UrlEncrypted = settings.DefaultUrlEncrypted;
            }
        }
        return Result.Success<object>(room);
    }

    public async Task<Result> RecordClickAsync(int sessionId, int tenantId, int? userId, string? role, string? ipAddress, string? userAgent, CancellationToken ct)
    {
        var room = await videoRooms.GetBySessionAndTenantNoTrackAsync(sessionId, tenantId, ct);
        if (room is null) return Result.Failure(Error.NotFound("Video room not found."));
        await videoRoomClicks.Create(new VideoRoomClick
        {
            TenantId = tenantId,
            SessionId = sessionId,
            VideoRoomId = room.Id,
            ClickedByUserId = userId,
            ClickedByRole = role,
            IpAddress = ipAddress,
            UserAgent = userAgent
        }, ct);
        return Result.Success();
    }

    public async Task<Result<object>> GetClicksAsync(int sessionId, int tenantId, CancellationToken ct) =>
        Result.Success<object>(await videoRoomClicks.ListBySessionOrderedDescAsync(sessionId, tenantId, ct));

    public async Task<Result> UpsertDefaultSettingsAsync(int tenantId, string provider, string? url, CancellationToken ct)
    {
        if (!IsAllowedProvider(provider)) return Result.Failure(VideoRoomErrors.InvalidProvider);
        if (!string.IsNullOrWhiteSpace(url))
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var parsed) || parsed.Scheme != Uri.UriSchemeHttps)
                return Result.Failure(VideoRoomErrors.UrlNotHttps);
        }

        var settings = await defaultSettings.GetByTenantAsync(tenantId, ct);
        if (settings is null)
        {
            settings = new DefaultVideoProviderSettings
            {
                TenantId = tenantId,
                DefaultProvider = provider,
                DefaultUrlEncrypted = url,
                DefaultUrlHash = string.IsNullOrWhiteSpace(url) ? null : Hash(url)
            };
            await defaultSettings.Create(settings, ct);
        }
        else
        {
            settings.DefaultProvider = provider;
            settings.DefaultUrlEncrypted = url;
            settings.DefaultUrlHash = string.IsNullOrWhiteSpace(url) ? null : Hash(url);
            await defaultSettings.Update(settings, ct);
        }
        return Result.Success();
    }

    public async Task<Result<object>> GetDefaultSettingsAsync(int tenantId, CancellationToken ct)
    {
        var settings = await defaultSettings.GetByTenantAsync(tenantId, ct);
        return settings is null
            ? Result.Success<object>(new { tenantId, defaultProvider = "external" })
            : Result.Success<object>(settings);
    }

    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

    private static readonly HashSet<string> AllowedProviders = new(StringComparer.OrdinalIgnoreCase) { "external", "zoom", "google_meet" };

    private static bool IsAllowedProvider(string provider) => AllowedProviders.Contains(provider);

    private static Result ValidateVideoUrl(string provider, string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return Result.Failure(VideoRoomErrors.UrlSchemeNotAllowed);
        if (!IsAllowedProvider(provider)) return Result.Failure(VideoRoomErrors.InvalidProvider);
        if (!Uri.TryCreate(url, UriKind.Absolute, out var parsed)) return Result.Failure(VideoRoomErrors.UrlSchemeNotAllowed);
        if (parsed.Scheme != Uri.UriSchemeHttps) return Result.Failure(VideoRoomErrors.UrlNotHttps);
        return Result.Success();
    }
}
