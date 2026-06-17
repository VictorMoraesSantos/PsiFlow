using Auth.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;

namespace Auth.Application.Features.Auth.Commands.RecordConsent;

public sealed class RecordConsentCommandHandler : ICommandHandler<RecordConsentCommand>
{
    private readonly IConsentService _consent;
    private readonly IValidator<RecordConsentCommand> _validator;

    public RecordConsentCommandHandler(IConsentService consent, IValidator<RecordConsentCommand> validator)
    {
        _consent = consent;
        _validator = validator;
    }

    public async Task<Result> Handle(RecordConsentCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            var failure = Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage)));
            var failureResult = Result.Failure(failure);
            return failureResult;
        }

        var successResult = await _consent.RecordAsync(command.UserId, command.Consent, cancellationToken);
        return successResult;
    }
}
