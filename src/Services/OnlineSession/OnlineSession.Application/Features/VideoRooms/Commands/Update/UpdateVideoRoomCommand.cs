using BuildingBlocks.CQRS.Requests.Commands;
using OnlineSession.Application.DTOs.VideoRoom;

namespace OnlineSession.Application.Features.VideoRooms.Commands.Update;

public sealed record UpdateVideoRoomCommand(int Id, UpdateVideoRoomDTO VideoRoom) : ICommand<bool>;
