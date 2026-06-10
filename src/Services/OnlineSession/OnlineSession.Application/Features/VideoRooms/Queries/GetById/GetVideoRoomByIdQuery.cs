using BuildingBlocks.CQRS.Requests.Queries;
using OnlineSession.Application.DTOs.VideoRoom;

namespace OnlineSession.Application.Features.VideoRooms.Queries.GetById;

public sealed record GetVideoRoomByIdQuery(int Id) : IQuery<VideoRoomDTO?>;
