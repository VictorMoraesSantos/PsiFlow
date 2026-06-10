using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using OnlineSession.Application.Contracts;
using OnlineSession.Application.DTOs.VideoRoom;

namespace OnlineSession.Application.Features.VideoRooms.Queries.GetById;

public sealed class GetVideoRoomByIdQueryHandler(IVideoRoomService service) : IQueryHandler<GetVideoRoomByIdQuery, VideoRoomDTO?>
{
    public Task<Result<VideoRoomDTO?>> Handle(GetVideoRoomByIdQuery query, CancellationToken cancellationToken) =>
        service.GetByIdAsync(query.Id, cancellationToken);
}
