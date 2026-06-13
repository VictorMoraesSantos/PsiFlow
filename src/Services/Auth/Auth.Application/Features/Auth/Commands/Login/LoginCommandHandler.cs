using Auth.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;

namespace Auth.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, object>
{
    private readonly IAuthService _service;
    private readonly IValidator<LoginCommand> _validator;

    public LoginCommandHandler(IAuthService service, IValidator<LoginCommand> validator)
    {
        _service = service;
        _validator = validator;
    }

    public async Task<Result<object>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure<object>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));

        return await _service.LoginAsync(command.Credentials, cancellationToken);
    }
}
