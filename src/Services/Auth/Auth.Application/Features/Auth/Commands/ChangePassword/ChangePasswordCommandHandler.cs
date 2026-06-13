using Auth.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;

namespace Auth.Application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand>
{
    private readonly IAuthService _service;

    public ChangePasswordCommandHandler(IAuthService service)
    {
        _service = service;
    }

    public Task<Result> Handle(ChangePasswordCommand command, CancellationToken cancellationToken) =>
        _service.ChangePasswordAsync(command.UserId, command.Password, cancellationToken);
}
