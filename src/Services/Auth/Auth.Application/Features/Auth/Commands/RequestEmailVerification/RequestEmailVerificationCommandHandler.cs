using Auth.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;

namespace Auth.Application.Features.Auth.Commands.RequestEmailVerification;

public sealed class RequestEmailVerificationCommandHandler : ICommandHandler<RequestEmailVerificationCommand, string>
{
    private readonly IEmailVerificationService _emailVerification;

    public RequestEmailVerificationCommandHandler(IEmailVerificationService emailVerification)
    {
        _emailVerification = emailVerification;
    }

    public async Task<Result<string>> Handle(RequestEmailVerificationCommand command, CancellationToken cancellationToken)
    {
        var result = await _emailVerification.RequestAsync(command.Email, cancellationToken);
        return result;
    }
}
