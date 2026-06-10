using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using OnlineSession.Application.Contracts;
using OnlineSession.Application.DTOs.VideoRoom;

namespace OnlineSession.Application.Features.VideoRooms.Queries.GetAll;

public sealed class GetVideoRoomsQueryHandler(IVideoRoomService service) : IQueryHandler<GetVideoRoomsQuery, IEnumerable<VideoRoomDTO?>>
{
    public Task<Result<IEnumerable<VideoRoomDTO?>>> Handle(GetVideoRoomsQuery query, CancellationToken cancellationToken) =>
        service.GetAllAsync(cancellationToken);
}
