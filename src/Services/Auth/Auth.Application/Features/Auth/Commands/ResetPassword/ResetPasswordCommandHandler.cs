using Auth.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;

namespace Auth.Application.Features.Auth.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand>
{
    private readonly IAuthService _service;
    private readonly IValidator<ResetPasswordCommand> _validator;

    public ResetPasswordCommandHandler(IAuthService service, IValidator<ResetPasswordCommand> validator)
    {
        _service = service;
        _validator = validator;
    }

    public async Task<Result> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));

        return await _service.ResetPasswordAsync(command.Data, cancellationToken);
    }
}
