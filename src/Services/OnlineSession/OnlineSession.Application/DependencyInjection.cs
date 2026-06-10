using BuildingBlocks.CQRS.Extensions;
using BuildingBlocks.Results;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OnlineSession.Application.DTOs.VideoRoom;
using OnlineSession.Application.Features.VideoRooms.Commands.Create;
using OnlineSession.Application.Features.VideoRooms.Commands.Delete;
using OnlineSession.Application.Features.VideoRooms.Commands.Update;
using OnlineSession.Application.Features.VideoRooms.Queries.GetAll;
using OnlineSession.Application.Features.VideoRooms.Queries.GetById;
using OnlineSession.Application.Features.Workflow;

namespace OnlineSession.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddOnlineSessionApplication(this IServiceCollection services)
        {
            services.AddMediatorService()
                .AddHandler<CreateVideoRoomCommand, Result<CreateVideoRoomResult>, CreateVideoRoomCommandHandler>()
                .AddHandler<UpdateVideoRoomCommand, Result<bool>, UpdateVideoRoomCommandHandler>()
                .AddHandler<DeleteVideoRoomCommand, Result<bool>, DeleteVideoRoomCommandHandler>()
                .AddHandler<GetVideoRoomsQuery, Result<IEnumerable<VideoRoomDTO?>>, GetVideoRoomsQueryHandler>()
                .AddHandler<GetVideoRoomByIdQuery, Result<VideoRoomDTO?>, GetVideoRoomByIdQueryHandler>()
                .AddHandler<UpsertVideoRoomCommand, Result<object>, UpsertVideoRoomCommandHandler>()
                .AddHandler<GetVideoRoomQuery, Result<object>, GetVideoRoomQueryHandler>()
                .AddHandler<RecordVideoRoomClickCommand, Result, RecordVideoRoomClickCommandHandler>()
                .AddHandler<GetVideoRoomClicksQuery, Result<object>, GetVideoRoomClicksQueryHandler>()
                .AddHandler<UpsertDefaultVideoSettingsCommand, Result, UpsertDefaultVideoSettingsCommandHandler>()
                .AddHandler<GetDefaultVideoSettingsQuery, Result<object>, GetDefaultVideoSettingsQueryHandler>();

            services.AddScoped<IValidator<CreateVideoRoomCommand>, CreateVideoRoomCommandValidator>();
            services.AddScoped<IValidator<UpdateVideoRoomCommand>, UpdateVideoRoomCommandValidator>();
            return services;
        }
    }
}
