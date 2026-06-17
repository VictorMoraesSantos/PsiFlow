using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;

namespace Auth.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler : ICommandHandler<RegisterCommand, RegisterResult>
{
    private readonly IRegistrationService _registration;
    private readonly IValidator<RegisterCommand> _validator;

    public RegisterCommandHandler(IRegistrationService registration, IValidator<RegisterCommand> validator)
    {
        _registration = registration;
        _validator = validator;
    }

    public async Task<Result<RegisterResult>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            var failure = Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage)));
            var failureResult = Result.Failure<RegisterResult>(failure);
            return failureResult;
        }

        var successResult = await _registration.RegisterAsync(command.Data, cancellationToken);
        return successResult;
    }
}
