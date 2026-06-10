using BuildingBlocks.CQRS.Requests.Commands;

namespace OnlineSession.Application.Features.VideoRooms.Commands.Delete;

public sealed record DeleteVideoRoomCommand(int Id) : ICommand<bool>;
