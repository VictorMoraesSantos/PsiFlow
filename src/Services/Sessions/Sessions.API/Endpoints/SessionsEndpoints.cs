using BuildingBlocks.CQRS.Sender;
using Sessions.Application.DTOs.Session;
using Sessions.Application.Features.Sessions.Commands.Create;
using Sessions.Application.Features.Sessions.Commands.Delete;
using Sessions.Application.Features.Sessions.Commands.Update;
using Sessions.Application.Features.Sessions.Queries.GetAll;
using Sessions.Application.Features.Sessions.Queries.GetById;
using static BuildingBlocks.Authorization.Policies;

namespace Sessions.API.Endpoints
{
    public static class SessionsEndpoints
    {
        public static IEndpointRouteBuilder MapSessionsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/v1/sessions")
                .WithTags("Sessions");

            group.MapGet("/", async (ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetSessionsQuery(), ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.Problem(result.Error!.Description, statusCode: StatusCodes.Status500InternalServerError);
            }).RequireAuthorization(Permissions.Sessions.View);

            group.MapGet("/{id:int}", async (int id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetSessionByIdQuery(id), ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.NotFound(new { error = result.Error!.Description });
            }).RequireAuthorization(Permissions.Sessions.View);

            group.MapPost("/", async (CreateSessionDTO dto, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new CreateSessionCommand(dto), ct);
                return result.IsSuccess
                    ? Results.Created($"/v1/sessions/{result.Value!.Id}", result.Value)
                    : Results.BadRequest(new { error = result.Error!.Description });
            }).RequireAuthorization(Permissions.Sessions.Create);

            group.MapPut("/{id:int}", async (int id, UpdateSessionDTO dto, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new UpdateSessionCommand(id, dto), ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error!.Description });
            }).RequireAuthorization(Permissions.Sessions.Edit);

            group.MapDelete("/{id:int}", async (int id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new DeleteSessionCommand(id), ct);
                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.NotFound(new { error = result.Error!.Description });
            }).RequireAuthorization(Permissions.Sessions.Delete);

            return app;
        }
    }
}
