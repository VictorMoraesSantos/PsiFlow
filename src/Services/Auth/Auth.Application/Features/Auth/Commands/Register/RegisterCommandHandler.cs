using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;

namespace Auth.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler : ICommandHandler<RegisterCommand, RegisterResult>
{
    private readonly IAuthService _service;
    private readonly IValidator<RegisterCommand> _validator;

    public RegisterCommandHandler(IAuthService service, IValidator<RegisterCommand> validator)
    {
        _service = service;
        _validator = validator;
    }

    public async Task<Result<RegisterResult>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure<RegisterResult>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));

        return await _service.RegisterAsync(command.Data, cancellationToken);
    }
}
