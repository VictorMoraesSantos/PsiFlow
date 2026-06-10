using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;
using Sessions.Application.Contracts;

namespace Sessions.Application.Features.Sessions.Commands.Create;

public sealed class CreateSessionCommandHandler : ICommandHandler<CreateSessionCommand, CreateSessionResult>
{
    private readonly ISessionService _service;
    private readonly IValidator<CreateSessionCommand> _validator;

    public CreateSessionCommandHandler(ISessionService service, IValidator<CreateSessionCommand> validator)
    {
        _service = service;
        _validator = validator;
    }

    public async Task<Result<CreateSessionResult>> Handle(CreateSessionCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure<CreateSessionResult>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));

        var result = await _service.CreateAsync(command.Session, cancellationToken);
        return result.IsSuccess
            ? Result.Success(new CreateSessionResult(result.Value))
            : Result.Failure<CreateSessionResult>(result.Error!);
    }
}
