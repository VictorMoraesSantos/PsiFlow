using Auth.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;

namespace Auth.Application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand>
{
    private readonly IPasswordService _passwords;
    private readonly IValidator<ChangePasswordCommand> _validator;

    public ChangePasswordCommandHandler(IPasswordService passwords, IValidator<ChangePasswordCommand> validator)
    {
        _passwords = passwords;
        _validator = validator;
    }

    public async Task<Result> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            var failure = Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage)));
            var failureResult = Result.Failure(failure);
            return failureResult;
        }

        var result = await _passwords.ChangeAsync(command.UserId, command.Password, cancellationToken);
        return result;
    }
}
