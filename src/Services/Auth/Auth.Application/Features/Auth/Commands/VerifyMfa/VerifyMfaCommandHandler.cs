using Auth.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;

namespace Auth.Application.Features.Auth.Commands.VerifyMfa;

public sealed class VerifyMfaCommandHandler : ICommandHandler<VerifyMfaCommand>
{
    private readonly IMfaService _mfa;
    private readonly IValidator<VerifyMfaCommand> _validator;

    public VerifyMfaCommandHandler(IMfaService mfa, IValidator<VerifyMfaCommand> validator)
    {
        _mfa = mfa;
        _validator = validator;
    }

    public async Task<Result> Handle(VerifyMfaCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            var failure = Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage)));
            var failureResult = Result.Failure(failure);
            return failureResult;
        }

        var successResult = await _mfa.VerifyAsync(command.UserId, command.Code.Code, cancellationToken);
        return successResult;
    }
}
