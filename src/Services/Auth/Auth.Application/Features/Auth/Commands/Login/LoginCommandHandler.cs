using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;

namespace Auth.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, object>
{
    private readonly ICredentialService _credentials;
    private readonly IMfaService _mfa;
    private readonly ITokenIssuanceService _tokens;
    private readonly IUserLifecycleService _userLifecycle;
    private readonly IValidator<LoginCommand> _validator;

    public LoginCommandHandler(
        ICredentialService credentials,
        IMfaService mfa,
        ITokenIssuanceService tokens,
        IUserLifecycleService userLifecycle,
        IValidator<LoginCommand> validator)
    {
        _credentials = credentials;
        _mfa = mfa;
        _tokens = tokens;
        _userLifecycle = userLifecycle;
        _validator = validator;
    }

    public async Task<Result<object>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure<object>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));

        var auth = await _credentials.AuthenticateAsync(command.Credentials.Email, command.Credentials.Password, cancellationToken);
        if (!auth.IsSuccess) return Result.Failure<object>(auth.Error!);

        var user = auth.Value!.User;

        if (auth.Value.RequiresMfa)
        {
            var challenge = await _mfa.StartLoginChallengeAsync(user, cancellationToken);
            if (!challenge.IsSuccess) return Result.Failure<object>(challenge.Error!);
            return Result.Success<object>(new MfaRequiredResponse(challenge.Value.MfaToken, challenge.Value.ChallengeId.ToString()));
        }

        await _userLifecycle.BeginLoginAsync(user, cancellationToken);
        if (user.Role == UserRole.Psychologist && user.TenantId.Value == 0)
            await _userLifecycle.AttachTenantAsync(user, user.Id, cancellationToken);

        var tokens = await _tokens.IssueAsync(user, cancellationToken);
        return tokens.IsSuccess
            ? Result.Success<object>(tokens.Value!)
            : Result.Failure<object>(tokens.Error!);
    }
}
