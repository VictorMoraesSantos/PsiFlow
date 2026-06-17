using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;

namespace Auth.Application.Features.Auth.Commands.CompleteMfaLogin;

public sealed class CompleteMfaLoginCommandHandler : ICommandHandler<CompleteMfaLoginCommand, TokenResponse>
{
    private readonly IMfaService _mfa;
    private readonly ITokenIssuanceService _tokens;
    private readonly IUserLifecycleService _userLifecycle;

    public CompleteMfaLoginCommandHandler(
        IMfaService mfa,
        ITokenIssuanceService tokens,
        IUserLifecycleService userLifecycle)
    {
        _mfa = mfa;
        _tokens = tokens;
        _userLifecycle = userLifecycle;
    }

    public async Task<Result<TokenResponse>> Handle(CompleteMfaLoginCommand command, CancellationToken cancellationToken)
    {
        var mfaResult = await _mfa.CompleteLoginChallengeAsync(command.MfaToken, command.Code, cancellationToken);
        if (!mfaResult.IsSuccess)
            return Result.Failure<TokenResponse>(mfaResult.Error!);

        var user = mfaResult.Value!.User;
        var beginLoginResult = await _userLifecycle.BeginLoginAsync(user, cancellationToken);
        var tokens = await _tokens.IssueAsync(user, cancellationToken);

        return tokens;
    }
}
