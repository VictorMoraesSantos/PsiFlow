using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;
using OnlineSession.Application.Contracts;

namespace OnlineSession.Application.Features.VideoRooms.Commands.Create;

public sealed class CreateVideoRoomCommandHandler : ICommandHandler<CreateVideoRoomCommand, CreateVideoRoomResult>
{
    private readonly IVideoRoomService _service;
    private readonly IValidator<CreateVideoRoomCommand> _validator;

    public CreateVideoRoomCommandHandler(IVideoRoomService service, IValidator<CreateVideoRoomCommand> validator)
    {
        _service = service;
        _validator = validator;
    }

    public async Task<Result<CreateVideoRoomResult>> Handle(CreateVideoRoomCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure<CreateVideoRoomResult>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));

        var result = await _service.CreateAsync(command.VideoRoom, cancellationToken);
        return result.IsSuccess
            ? Result.Success(new CreateVideoRoomResult(result.Value))
            : Result.Failure<CreateVideoRoomResult>(result.Error!);
    }
}
