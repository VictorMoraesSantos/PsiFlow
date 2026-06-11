using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using OnlineSession.Application.Contracts;

namespace OnlineSession.Application.Features.Workflow;

public sealed class UpsertDefaultVideoSettingsCommandHandler(IOnlineSessionService service) : ICommandHandler<UpsertDefaultVideoSettingsCommand>
{
    public Task<Result> Handle(UpsertDefaultVideoSettingsCommand command, CancellationToken cancellationToken) =>
        service.UpsertDefaultSettingsAsync(command.TenantId, command.Provider, command.Url, cancellationToken);
}
