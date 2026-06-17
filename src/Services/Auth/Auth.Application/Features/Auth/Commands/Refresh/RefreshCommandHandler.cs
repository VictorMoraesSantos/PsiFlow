using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;

namespace Auth.Application.Features.Auth.Commands.Refresh;

public sealed class RefreshCommandHandler : ICommandHandler<RefreshCommand, TokenResponse>
{
    private readonly ITokenIssuanceService _tokens;

    public RefreshCommandHandler(ITokenIssuanceService tokens)
    {
        _tokens = tokens;
    }

    public async Task<Result<TokenResponse>> Handle(RefreshCommand command, CancellationToken cancellationToken)
    {
        var result = await _tokens.RefreshAsync(command.RefreshToken, cancellationToken);
        return result;
    }
}
