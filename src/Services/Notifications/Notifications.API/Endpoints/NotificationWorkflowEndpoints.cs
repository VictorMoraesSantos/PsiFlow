using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Notifications.Application.Features.Workflow;
using System.Security.Claims;
using static BuildingBlocks.Authorization.Policies;

namespace Notifications.API.Endpoints;

public static class NotificationWorkflowEndpoints
{
    public static IEndpointRouteBuilder MapNotificationWorkflowEndpoints(this IEndpointRouteBuilder app)
    {
        var tpl = app.MapGroup("/v1/notification-templates").WithTags("NotificationTemplates");
        tpl.MapPost("/{templateId:int}/versions", async (int templateId, TemplateVersionBody body, ISender sender, HttpContext http, CancellationToken ct) => { var r = await sender.Send(new CreateTemplateVersionCommand(templateId, body.Subject, body.BodyHtml, body.BodyText, http.GetUserId()), ct); return r.IsSuccess ? Results.Created($"/v1/notification-templates/{templateId}/versions", r.Value) : ToHttp(r); }).RequireAuthorization(Permissions.Notifications.Create);
        var log = app.MapGroup("/v1/notification-logs").WithTags("NotificationLogs");
        log.MapGet("/", async (ISender sender, CancellationToken ct) => ToHttp(await sender.Send(new GetNotificationLogsQuery(), ct))).RequireAuthorization(Permissions.Notifications.View);
        log.MapGet("/{notificationId:int}", async (int notificationId, ISender sender, CancellationToken ct) => ToHttp(await sender.Send(new GetNotificationLogQuery(notificationId), ct))).RequireAuthorization(Permissions.Notifications.View);
        var ops = app.MapGroup("/v1/notifications").WithTags("Notifications");
        ops.MapPost("/test-email", async (TestEmailBody body, ISender sender, HttpContext http, CancellationToken ct) => ToHttp(await sender.Send(new SendTestEmailCommand(body.RecipientEmail, body.TemplateKey, http.GetTenantId()), ct))).RequireAuthorization(Permissions.Notifications.Create);
        ops.MapPost("/retry/{notificationId:int}", async (int notificationId, ISender sender, CancellationToken ct) => ToHttp(await sender.Send(new RetryNotificationCommand(notificationId), ct), StatusCodes.Status204NoContent)).RequireAuthorization(Permissions.Notifications.Edit);
        ops.MapPost("/schedule-reminders", async (ScheduleReminderBody body, ISender sender, HttpContext http, CancellationToken ct) => ToHttp(await sender.Send(new ScheduleReminderCommand(body.NotificationType, body.ScheduledFor, body.RecipientEmail, body.RecipientUserId, http.GetTenantId(), body.PayloadJson), ct))).RequireAuthorization(Permissions.Notifications.Create);
        return app;
    }

    private static IResult ToHttp(Result result, int successStatus = StatusCodes.Status200OK) => result.IsSuccess ? successStatus == StatusCodes.Status204NoContent ? Results.NoContent() : Results.Ok() : result.Error!.Type == ErrorType.NotFound ? Results.NotFound(new { error = result.Error.Description }) : Results.BadRequest(new { error = result.Error.Description });
    private static IResult ToHttp<T>(Result<T> result) => result.IsSuccess ? Results.Ok(result.Value) : result.Error!.Type == ErrorType.NotFound ? Results.NotFound(new { error = result.Error.Description }) : Results.BadRequest(new { error = result.Error.Description });
}

internal static class NotificationsHttpContextExtensions
{
    public static int? GetTenantId(this HttpContext http) => int.TryParse(http.User.FindFirstValue("tenant_id"), out var id) ? id : null;
    public static int GetUserId(this HttpContext http) => int.TryParse(http.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? http.User.FindFirstValue("sub"), out var id) ? id : 0;
}
