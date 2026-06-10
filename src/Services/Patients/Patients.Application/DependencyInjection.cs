using BuildingBlocks.CQRS.Extensions;
using BuildingBlocks.Results;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Patients.Application.DTOs.Patient;
using Patients.Application.Features.Patients.Commands.Create;
using Patients.Application.Features.Patients.Commands.Delete;
using Patients.Application.Features.Patients.Commands.Update;
using Patients.Application.Features.Patients.Queries.GetAll;
using Patients.Application.Features.Patients.Queries.GetById;
using Patients.Application.Features.Workflow;

namespace Patients.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPatientsApplication(this IServiceCollection services)
        {
            services.AddMediatorService()
                .AddHandler<CreatePatientCommand, Result<CreatePatientResult>, CreatePatientCommandHandler>()
                .AddHandler<UpdatePatientCommand, Result<bool>, UpdatePatientCommandHandler>()
                .AddHandler<DeletePatientCommand, Result<bool>, DeletePatientCommandHandler>()
                .AddHandler<GetPatientsQuery, Result<IEnumerable<PatientDTO?>>, GetPatientsQueryHandler>()
                .AddHandler<GetPatientByIdQuery, Result<PatientDTO?>, GetPatientByIdQueryHandler>()
                .AddHandler<DeactivatePatientCommand, Result, DeactivatePatientCommandHandler>()
                .AddHandler<ChangeTreatmentStatusCommand, Result<object>, ChangeTreatmentStatusCommandHandler>()
                .AddHandler<GetPatientSessionsSummaryQuery, Result<object>, GetPatientSessionsSummaryQueryHandler>()
                .AddHandler<CreatePatientInviteCommand, Result<object>, CreatePatientInviteCommandHandler>()
                .AddHandler<PreviewPatientInviteQuery, Result<object>, PreviewPatientInviteQueryHandler>()
                .AddHandler<AcceptPatientInviteCommand, Result<object>, AcceptPatientInviteCommandHandler>()
                .AddHandler<RevokePatientInviteCommand, Result, RevokePatientInviteCommandHandler>();

            services.AddScoped<IValidator<CreatePatientCommand>, CreatePatientCommandValidator>();
            services.AddScoped<IValidator<UpdatePatientCommand>, UpdatePatientCommandValidator>();
            services.AddScoped<IValidator<CreatePatientInviteCommand>, CreatePatientInviteCommandValidator>();
            services.AddScoped<IValidator<ChangeTreatmentStatusCommand>, ChangeTreatmentStatusCommandValidator>();
            return services;
        }
    }
}
