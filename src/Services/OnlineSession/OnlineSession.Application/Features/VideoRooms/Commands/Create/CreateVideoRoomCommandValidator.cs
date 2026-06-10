using FluentValidation;

namespace OnlineSession.Application.Features.VideoRooms.Commands.Create;

public sealed class CreateVideoRoomCommandValidator : AbstractValidator<CreateVideoRoomCommand>
{
    public CreateVideoRoomCommandValidator()
    {
        RuleFor(command => command.VideoRoom.TenantId).GreaterThan(0);
        RuleFor(command => command.VideoRoom.SessionId).GreaterThan(0);
        RuleFor(command => command.VideoRoom.Name).NotEmpty().MaximumLength(200);
    }
}
