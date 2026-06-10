using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;
using OnlineSession.Application.Contracts;

namespace OnlineSession.Application.Features.VideoRooms.Commands.Update;

public sealed class UpdateVideoRoomCommandHandler : ICommandHandler<UpdateVideoRoomCommand, bool>
{
    private readonly IVideoRoomService _service;
    private readonly IValidator<UpdateVideoRoomCommand> _validator;

    public UpdateVideoRoomCommandHandler(IVideoRoomService service, IValidator<UpdateVideoRoomCommand> validator)
    {
        _service = service;
        _validator = validator;
    }

    public async Task<Result<bool>> Handle(UpdateVideoRoomCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure<bool>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));

        return await _service.UpdateAsync(command.VideoRoom with { Id = command.Id }, cancellationToken);
    }
}
