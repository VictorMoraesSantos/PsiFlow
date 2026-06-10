using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Sessions.Application.Contracts;

namespace Sessions.Application.Features.Sessions.Commands.Delete;

public sealed class DeleteSessionCommandHandler(ISessionService service) : ICommandHandler<DeleteSessionCommand, bool>
{
    public Task<Result<bool>> Handle(DeleteSessionCommand command, CancellationToken cancellationToken) =>
        service.DeleteAsync(command.Id, cancellationToken);
}
