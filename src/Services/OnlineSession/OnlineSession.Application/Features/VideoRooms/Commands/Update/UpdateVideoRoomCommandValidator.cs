using FluentValidation;

namespace OnlineSession.Application.Features.VideoRooms.Commands.Update;

public sealed class UpdateVideoRoomCommandValidator : AbstractValidator<UpdateVideoRoomCommand>
{
    public UpdateVideoRoomCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.VideoRoom.TenantId).GreaterThan(0);
        RuleFor(command => command.VideoRoom.SessionId).GreaterThan(0);
        RuleFor(command => command.VideoRoom.Name).NotEmpty().MaximumLength(200);
    }
}
