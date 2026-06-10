namespace PsiFlow.Notifications.API.Endpoints;

public static class NotificationsEndpoints
{
    public static IEndpointRouteBuilder MapNotificationsEndpoints(this IEndpointRouteBuilder app)
    {
        var tpl = app.MapGroup("/v1/notification-templates").WithTags("NotificationTemplates").RequireAuthorization("RequireSaasAdmin");
        tpl.MapGet("/", () => Results.Ok(Array.Empty<object>()));
        tpl.MapPost("/", () => Results.Created($"/v1/notification-templates/{Guid.NewGuid()}", new { }));
        tpl.MapPost("/{templateId:guid}/versions", (Guid id) => Results.Created($"/v1/notification-templates/{id}/versions/{1}", new { }));

        var log = app.MapGroup("/v1/notification-logs").WithTags("NotificationLogs").RequireAuthorization("RequireSaasAdmin");
        log.MapGet("/", () => Results.Ok(Array.Empty<object>()));
        log.MapGet("/{notificationId:guid}", (Guid id) => Results.Ok(new { id }));

        var ops = app.MapGroup("/v1/notifications").WithTags("Notifications").RequireAuthorization("RequireSaasAdmin");
        ops.MapPost("/test-email", () => Results.Accepted());
        ops.MapPost("/retry/{notificationId:guid}", (Guid id) => Results.NoContent());

        return app;
    }
}
