using Auth.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;

namespace Auth.Application.Features.Auth.Commands.Logout;

public sealed class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly IAuthService _service;

    public LogoutCommandHandler(IAuthService service)
    {
        _service = service;
    }

    public Task<Result> Handle(LogoutCommand command, CancellationToken cancellationToken) =>
        _service.LogoutAsync(command.UserId, cancellationToken);
}
