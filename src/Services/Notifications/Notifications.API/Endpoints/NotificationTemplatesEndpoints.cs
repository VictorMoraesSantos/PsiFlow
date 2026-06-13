using BuildingBlocks.Authorization;
using BuildingBlocks.CQRS.Sender;
using Notifications.Application.DTOs.NotificationTemplate;
using Notifications.Application.Features.NotificationTemplates.Commands.Create;
using Notifications.Application.Features.NotificationTemplates.Commands.Delete;
using Notifications.Application.Features.NotificationTemplates.Commands.Update;
using Notifications.Application.Features.NotificationTemplates.Queries.GetAll;
using Notifications.Application.Features.NotificationTemplates.Queries.GetById;
using static BuildingBlocks.Authorization.Policies;

namespace Notifications.API.Endpoints
{
    public static class NotificationTemplatesEndpoints
    {
        public static IEndpointRouteBuilder MapNotificationTemplatesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/v1/notification-templates")
                .WithTags("NotificationTemplates");

            group.MapGet("/", async (ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetNotificationTemplatesQuery(), ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.Problem(result.Error!.Description, statusCode: StatusCodes.Status500InternalServerError);
            }).RequireAuthorization(Permissions.Notifications.View);

            group.MapGet("/{id:int}", async (int id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetNotificationTemplateByIdQuery(id), ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.NotFound(new { error = result.Error!.Description });
            }).RequireAuthorization(Permissions.Notifications.View);

            group.MapPost("/", async (CreateNotificationTemplateDTO dto, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new CreateNotificationTemplateCommand(dto), ct);
                return result.IsSuccess
                    ? Results.Created($"/v1/notification-templates/{result.Value!.Id}", result.Value)
                    : Results.BadRequest(new { error = result.Error!.Description });
            }).RequireAuthorization(Permissions.Notifications.Create);

            group.MapPut("/{id:int}", async (int id, UpdateNotificationTemplateDTO dto, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new UpdateNotificationTemplateCommand(id, dto), ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error!.Description });
            }).RequireAuthorization(Permissions.Notifications.Edit);

            group.MapDelete("/{id:int}", async (int id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new DeleteNotificationTemplateCommand(id), ct);
                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.NotFound(new { error = result.Error!.Description });
            }).RequireAuthorization(Permissions.Notifications.Delete);

            return app;
        }
    }
}
