using Auth.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;

namespace Auth.Application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand>
{
    private readonly IPasswordService _passwords;

    public ChangePasswordCommandHandler(IPasswordService passwords)
    {
        _passwords = passwords;
    }

    public async Task<Result> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await _passwords.ChangeAsync(command.UserId, command.Password, cancellationToken);
        return result;
    }
}
