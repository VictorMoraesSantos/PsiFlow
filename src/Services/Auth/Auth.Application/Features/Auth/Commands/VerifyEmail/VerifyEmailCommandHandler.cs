using Auth.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;

namespace Auth.Application.Features.Auth.Commands.VerifyEmail;

public sealed class VerifyEmailCommandHandler : ICommandHandler<VerifyEmailCommand>
{
    private readonly IEmailVerificationService _emailVerification;

    public VerifyEmailCommandHandler(IEmailVerificationService emailVerification)
    {
        _emailVerification = emailVerification;
    }

    public Task<Result> Handle(VerifyEmailCommand command, CancellationToken cancellationToken) =>
        _emailVerification.VerifyAsync(command.Email, command.Token, cancellationToken);
}
