using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;

namespace Auth.Application.Features.Auth.Commands.SetupMfa;

public sealed class SetupMfaCommandHandler : ICommandHandler<SetupMfaCommand, MfaSetupResult>
{
    private readonly IAuthService _service;

    public SetupMfaCommandHandler(IAuthService service)
    {
        _service = service;
    }

    public Task<Result<MfaSetupResult>> Handle(SetupMfaCommand command, CancellationToken cancellationToken) =>
        _service.SetupMfaAsync(command.UserId, cancellationToken);
}
