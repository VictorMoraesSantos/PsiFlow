using Agenda.Application.Contracts;
using Agenda.Application.DTOs.Appointment;
using Agenda.Application.Features.Appointments.Commands.Create;
using Agenda.Application.Features.Appointments.Commands.Delete;
using Agenda.Application.Features.Appointments.Commands.Update;
using Agenda.Application.Features.Appointments.Queries.GetAll;
using Agenda.Application.Features.Appointments.Queries.GetById;
using Agenda.Application.Features.Workflow;
using BuildingBlocks.CQRS.Extensions;
using BuildingBlocks.Results;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Agenda.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAgendaApplication(this IServiceCollection services)
        {
            services.AddMediatorService()
                .AddHandler<CreateAppointmentCommand, Result<CreateAppointmentResult>, CreateAppointmentCommandHandler>()
                .AddHandler<UpdateAppointmentCommand, Result<bool>, UpdateAppointmentCommandHandler>()
                .AddHandler<DeleteAppointmentCommand, Result<bool>, DeleteAppointmentCommandHandler>()
                .AddHandler<GetAppointmentsQuery, Result<IEnumerable<AppointmentDTO?>>, GetAppointmentsQueryHandler>()
                .AddHandler<GetAppointmentByIdQuery, Result<AppointmentDTO?>, GetAppointmentByIdQueryHandler>()
                .AddHandler<CreateWeeklyAvailabilityCommand, Result<WeeklyAvailabilityResult>, CreateWeeklyAvailabilityCommandHandler>()
                .AddHandler<UpdateWeeklyAvailabilityCommand, Result<bool>, UpdateWeeklyAvailabilityCommandHandler>()
                .AddHandler<DeleteWeeklyAvailabilityCommand, Result<bool>, DeleteWeeklyAvailabilityCommandHandler>()
                .AddHandler<CreateScheduleBlockCommand, Result<ScheduleBlockResult>, CreateScheduleBlockCommandHandler>()
                .AddHandler<GetWeeklyAvailabilitiesQuery, Result<IReadOnlyCollection<WeeklyAvailabilityResult>>, GetWeeklyAvailabilitiesQueryHandler>()
                .AddHandler<GetScheduleBlocksQuery, Result<IReadOnlyCollection<ScheduleBlockResult>>, GetScheduleBlocksQueryHandler>()
                .AddHandler<DeleteScheduleBlockCommand, Result<bool>, DeleteScheduleBlockCommandHandler>()
                .AddHandler<GetAvailableSlotsQuery, Result<IReadOnlyCollection<AvailableSlotResult>>, GetAvailableSlotsQueryHandler>()
                .AddHandler<CancelAppointmentCommand, Result<bool>, CancelAppointmentCommandHandler>();

            services.AddScoped<IValidator<CreateAppointmentCommand>, CreateAppointmentCommandValidator>();
            services.AddScoped<IValidator<UpdateAppointmentCommand>, UpdateAppointmentCommandValidator>();
            services.AddScoped<IValidator<CreateWeeklyAvailabilityCommand>, CreateWeeklyAvailabilityCommandValidator>();
            services.AddScoped<IValidator<UpdateWeeklyAvailabilityCommand>, UpdateWeeklyAvailabilityCommandValidator>();
            services.AddScoped<IValidator<CreateScheduleBlockCommand>, CreateScheduleBlockCommandValidator>();
            services.AddScoped<IValidator<GetAvailableSlotsQuery>, GetAvailableSlotsQueryValidator>();
            return services;
        }
    }
}
