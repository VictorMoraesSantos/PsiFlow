using Auth.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;

namespace Auth.Application.Features.Auth.Commands.RecordConsent;

public sealed class RecordConsentCommandHandler : ICommandHandler<RecordConsentCommand>
{
    private readonly IAuthService _service;
    private readonly IValidator<RecordConsentCommand> _validator;

    public RecordConsentCommandHandler(IAuthService service, IValidator<RecordConsentCommand> validator)
    {
        _service = service;
        _validator = validator;
    }

    public async Task<Result> Handle(RecordConsentCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));

        return await _service.RecordConsentAsync(command.UserId, command.Consent, cancellationToken);
    }
}
