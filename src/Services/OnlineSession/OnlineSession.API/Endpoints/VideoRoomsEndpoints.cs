using BuildingBlocks.CQRS.Sender;
using OnlineSession.Application.DTOs.VideoRoom;
using OnlineSession.Application.Features.VideoRooms.Commands.Create;
using OnlineSession.Application.Features.VideoRooms.Commands.Delete;
using OnlineSession.Application.Features.VideoRooms.Commands.Update;
using OnlineSession.Application.Features.VideoRooms.Queries.GetAll;
using OnlineSession.Application.Features.VideoRooms.Queries.GetById;

namespace OnlineSession.API.Endpoints
{
    public static class VideoRoomsEndpoints
    {
        public static IEndpointRouteBuilder MapVideoRoomsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/v1/video-rooms")
                .WithTags("VideoRooms")
                .RequireAuthorization();

            group.MapGet("/", async (ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetVideoRoomsQuery(), ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.Problem(result.Error!.Description, statusCode: StatusCodes.Status500InternalServerError);
            });

            group.MapGet("/{id:int}", async (int id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetVideoRoomByIdQuery(id), ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.NotFound(new { error = result.Error!.Description });
            });

            group.MapPost("/", async (CreateVideoRoomDTO dto, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new CreateVideoRoomCommand(dto), ct);
                return result.IsSuccess
                    ? Results.Created($"/v1/video-rooms/{result.Value!.Id}", result.Value)
                    : Results.BadRequest(new { error = result.Error!.Description });
            });

            group.MapPut("/{id:int}", async (int id, UpdateVideoRoomDTO dto, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new UpdateVideoRoomCommand(id, dto), ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error!.Description });
            });

            group.MapDelete("/{id:int}", async (int id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new DeleteVideoRoomCommand(id), ct);
                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.NotFound(new { error = result.Error!.Description });
            });

            return app;
        }
    }
}
