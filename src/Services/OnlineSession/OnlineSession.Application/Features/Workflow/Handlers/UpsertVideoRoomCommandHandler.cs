using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using OnlineSession.Application.Contracts;

namespace OnlineSession.Application.Features.Workflow;

public sealed class UpsertVideoRoomCommandHandler(IOnlineSessionService service) : ICommandHandler<UpsertVideoRoomCommand, object>
{
    public Task<Result<object>> Handle(UpsertVideoRoomCommand command, CancellationToken cancellationToken) =>
        service.UpsertVideoRoomAsync(command.SessionId, command.TenantId, command.UserId, command.Name, command.Provider, command.Url, command.Instructions, cancellationToken);
}
