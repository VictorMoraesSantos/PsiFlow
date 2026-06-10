using BuildingBlocks.CQRS.Requests.Commands;
using OnlineSession.Application.DTOs.VideoRoom;

namespace OnlineSession.Application.Features.VideoRooms.Commands.Create;

public sealed record CreateVideoRoomCommand(CreateVideoRoomDTO VideoRoom) : ICommand<CreateVideoRoomResult>;
public sealed record CreateVideoRoomResult(int Id);
