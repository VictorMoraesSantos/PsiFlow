using Auth.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;

namespace Auth.Application.Features.Auth.Commands.Logout;

public sealed class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly ITokenRevocationService _revocation;

    public LogoutCommandHandler(ITokenRevocationService revocation)
    {
        _revocation = revocation;
    }

    public async Task<Result> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        var result = await _revocation.RevokeAllForUserAsync(command.UserId, cancellationToken);
        return result;
    }
}
