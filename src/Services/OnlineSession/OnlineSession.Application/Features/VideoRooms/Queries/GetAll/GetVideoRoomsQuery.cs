using BuildingBlocks.CQRS.Requests.Queries;
using OnlineSession.Application.DTOs.VideoRoom;

namespace OnlineSession.Application.Features.VideoRooms.Queries.GetAll;

public sealed record GetVideoRoomsQuery : IQuery<IEnumerable<VideoRoomDTO?>>;
