using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using OnlineSession.Application.Contracts;

namespace OnlineSession.Application.Features.Workflow;

public sealed class RecordVideoRoomClickCommandHandler(IOnlineSessionService service) : ICommandHandler<RecordVideoRoomClickCommand>
{
    public Task<Result> Handle(RecordVideoRoomClickCommand command, CancellationToken cancellationToken) =>
        service.RecordClickAsync(command.SessionId, command.TenantId, command.UserId, command.Role, command.IpAddress, command.UserAgent, cancellationToken);
}
