using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using Auth.Domain.ValueObjects;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;

namespace Auth.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, object>
{
    private readonly ICredentialService _credentials;
    private readonly ITokenService _tokens;
    private readonly IUserService _users;
    private readonly IValidator<LoginCommand> _validator;

    public LoginCommandHandler(
        ICredentialService credentials,
        ITokenService tokens,
        IUserService users,
        IValidator<LoginCommand> validator)
    {
        _credentials = credentials;
        _tokens = tokens;
        _users = users;
        _validator = validator;
    }

    public async Task<Result<object>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            var failure = Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage)));
            var failureResult = Result.Failure<object>(failure);
            return failureResult;
        }

        var auth = await _credentials.AuthenticateAsync(command.Credentials.Email, command.Credentials.Password, cancellationToken);
        if (!auth.IsSuccess)
            return Result.Failure<object>(auth.Error!);

        var user = auth.Value!;

        await _users.BeginLoginAsync(user, cancellationToken);
        if (user.Role == UserRole.Psychologist && user.TenantId.Value == 0)
            await _users.AttachTenantAsync(user, user.Id, cancellationToken);

        var tokens = await _tokens.IssueAsync(user, cancellationToken);
        if (tokens.IsSuccess)
            return Result.Success<object>(tokens.Value!);

        return Result.Failure<object>(tokens.Error!);
    }
}
