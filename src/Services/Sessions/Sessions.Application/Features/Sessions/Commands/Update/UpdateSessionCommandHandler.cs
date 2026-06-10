using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;
using Sessions.Application.Contracts;

namespace Sessions.Application.Features.Sessions.Commands.Update;

public sealed class UpdateSessionCommandHandler : ICommandHandler<UpdateSessionCommand, bool>
{
    private readonly ISessionService _service;
    private readonly IValidator<UpdateSessionCommand> _validator;

    public UpdateSessionCommandHandler(ISessionService service, IValidator<UpdateSessionCommand> validator)
    {
        _service = service;
        _validator = validator;
    }

    public async Task<Result<bool>> Handle(UpdateSessionCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure<bool>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));

        return await _service.UpdateAsync(command.Session with { Id = command.Id }, cancellationToken);
    }
}
