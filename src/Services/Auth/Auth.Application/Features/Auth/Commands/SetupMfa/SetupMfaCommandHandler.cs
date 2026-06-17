using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;

namespace Auth.Application.Features.Auth.Commands.SetupMfa;

public sealed class SetupMfaCommandHandler : ICommandHandler<SetupMfaCommand, MfaSetupResult>
{
    private readonly IMfaService _mfa;

    public SetupMfaCommandHandler(IMfaService mfa)
    {
        _mfa = mfa;
    }

    public async Task<Result<MfaSetupResult>> Handle(SetupMfaCommand command, CancellationToken cancellationToken)
    {
        var result = await _mfa.SetupAsync(command.UserId, cancellationToken);
        return result;
    }
}
