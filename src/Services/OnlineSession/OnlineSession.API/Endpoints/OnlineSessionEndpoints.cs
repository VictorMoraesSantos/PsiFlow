using BuildingBlocks.Authorization;
using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using OnlineSession.Application.Features.Workflow;
using System.Security.Claims;
using static BuildingBlocks.Authorization.Policies;

namespace OnlineSession.API.Endpoints;

public static class OnlineSessionEndpoints
{
    public static IEndpointRouteBuilder MapOnlineSessionEndpoints(this IEndpointRouteBuilder app)
    {
        var grp = app.MapGroup("/v1/sessions/{sessionId:int}/video-room").WithTags("VideoRoom");
        grp.MapPut("/", async (int sessionId, VideoRoomBody body, ISender sender, HttpContext http, CancellationToken ct) => ToHttp(await sender.Send(new UpsertVideoRoomCommand(sessionId, http.GetTenantId(), http.GetUserId(), body.Name, body.Provider ?? "external", body.Url, body.Instructions), ct))).RequireAuthorization(Permissions.OnlineSession.Edit);
        grp.MapGet("/", async (int sessionId, ISender sender, HttpContext http, CancellationToken ct) => ToHttp(await sender.Send(new GetVideoRoomQuery(sessionId, http.GetTenantId()), ct))).RequireAuthorization(Permissions.OnlineSession.View);
        grp.MapPost("/click", async (int sessionId, ISender sender, HttpContext http, CancellationToken ct) => ToHttp(await sender.Send(new RecordVideoRoomClickCommand(sessionId, http.GetTenantId(), http.GetUserIdOrNull(), http.User.FindFirstValue(ClaimTypes.Role), http.Connection.RemoteIpAddress?.ToString(), http.Request.Headers.UserAgent.ToString()), ct), StatusCodes.Status204NoContent)).RequireAuthorization(Permissions.OnlineSession.Edit);
        grp.MapGet("/clicks", async (int sessionId, ISender sender, HttpContext http, CancellationToken ct) => ToHttp(await sender.Send(new GetVideoRoomClicksQuery(sessionId, http.GetTenantId()), ct))).RequireAuthorization(Permissions.OnlineSession.View);
        var settings = app.MapGroup("/v1/video-settings/default-link").WithTags("VideoSettings").RequireAuthorization(Permissions.OnlineSession.Edit);
        settings.MapPut("/", async (DefaultVideoSettingsBody body, ISender sender, HttpContext http, CancellationToken ct) => ToHttp(await sender.Send(new UpsertDefaultVideoSettingsCommand(http.GetTenantId(), body.Provider ?? "external", body.Url), ct), StatusCodes.Status204NoContent));
        settings.MapGet("/", async (ISender sender, HttpContext http, CancellationToken ct) => ToHttp(await sender.Send(new GetDefaultVideoSettingsQuery(http.GetTenantId()), ct)));
        return app;
    }

    private static IResult ToHttp(Result result, int successStatus = StatusCodes.Status200OK) => result.IsSuccess ? successStatus == StatusCodes.Status204NoContent ? Results.NoContent() : Results.Ok() : result.Error!.Type == ErrorType.NotFound ? Results.NotFound(new { error = result.Error.Description }) : Results.BadRequest(new { error = result.Error.Description });
    private static IResult ToHttp<T>(Result<T> result) => result.IsSuccess ? Results.Ok(result.Value) : result.Error!.Type == ErrorType.NotFound ? Results.NotFound(new { error = result.Error.Description }) : Results.BadRequest(new { error = result.Error.Description });
}

internal static class OnlineSessionHttpContextExtensions
{
    public static int GetTenantId(this HttpContext http) => int.TryParse(http.User.FindFirstValue("tenant_id"), out var id) ? id : 0;
    public static int GetUserId(this HttpContext http) => int.TryParse(http.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? http.User.FindFirstValue("sub"), out var id) ? id : 0;
    public static int? GetUserIdOrNull(this HttpContext http) => int.TryParse(http.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? http.User.FindFirstValue("sub"), out var id) ? id : null;
}
