using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using OnlineSession.Application.Contracts;

namespace OnlineSession.Application.Features.VideoRooms.Commands.Delete;

public sealed class DeleteVideoRoomCommandHandler(IVideoRoomService service) : ICommandHandler<DeleteVideoRoomCommand, bool>
{
    public Task<Result<bool>> Handle(DeleteVideoRoomCommand command, CancellationToken cancellationToken) =>
        service.DeleteAsync(command.Id, cancellationToken);
}
