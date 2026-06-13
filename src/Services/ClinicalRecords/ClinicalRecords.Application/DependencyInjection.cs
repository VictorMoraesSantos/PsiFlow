using BuildingBlocks.CQRS.Extensions;
using BuildingBlocks.Results;
using ClinicalRecords.Application.Contracts;
using ClinicalRecords.Application.DTOs.MedicalRecord;
using ClinicalRecords.Application.Features.MedicalRecords.Commands.Create;
using ClinicalRecords.Application.Features.MedicalRecords.Commands.Delete;
using ClinicalRecords.Application.Features.MedicalRecords.Commands.Update;
using ClinicalRecords.Application.Features.MedicalRecords.Queries.GetAll;
using ClinicalRecords.Application.Features.MedicalRecords.Queries.GetById;
using ClinicalRecords.Application.Features.Workflow;
using ClinicalRecords.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicalRecords.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddClinicalRecordsApplication(this IServiceCollection services)
        {
            services.AddScoped<IMedicalRecordService, MedicalRecordService>();
            services.AddScoped<IClinicalRecordWorkflowService, ClinicalRecordWorkflowService>();

            services.AddMediatorService()
                .AddHandler<CreateMedicalRecordCommand, Result<CreateMedicalRecordResult>, CreateMedicalRecordCommandHandler>()
                .AddHandler<UpdateMedicalRecordCommand, Result<bool>, UpdateMedicalRecordCommandHandler>()
                .AddHandler<DeleteMedicalRecordCommand, Result<bool>, DeleteMedicalRecordCommandHandler>()
                .AddHandler<GetMedicalRecordsQuery, Result<IEnumerable<MedicalRecordDTO?>>, GetMedicalRecordsQueryHandler>()
                .AddHandler<GetMedicalRecordByIdQuery, Result<MedicalRecordDTO?>, GetMedicalRecordByIdQueryHandler>()
                .AddHandler<GetClinicalRecordByPatientQuery, Result<object>, GetClinicalRecordByPatientQueryHandler>()
                .AddHandler<CreateClinicalRecordCommand, Result<object>, CreateClinicalRecordCommandHandler>()
                .AddHandler<GetClinicalRecordQuery, Result<object>, GetClinicalRecordQueryHandler>()
                .AddHandler<GetAnamnesisQuery, Result<object>, GetAnamnesisQueryHandler>()
                .AddHandler<AutosaveAnamnesisCommand, Result, AutosaveAnamnesisCommandHandler>()
                .AddHandler<PublishAnamnesisVersionCommand, Result<object>, PublishAnamnesisVersionCommandHandler>()
                .AddHandler<GetEvolutionQuery, Result<object>, GetEvolutionQueryHandler>()
                .AddHandler<AutosaveEvolutionCommand, Result, AutosaveEvolutionCommandHandler>()
                .AddHandler<PublishEvolutionVersionCommand, Result<object>, PublishEvolutionVersionCommandHandler>()
                .AddHandler<GetEvolutionVersionsQuery, Result<object>, GetEvolutionVersionsQueryHandler>()
                .AddHandler<GetClinicalAuditLogQuery, Result<object>, GetClinicalAuditLogQueryHandler>();

            services.AddScoped<IValidator<CreateMedicalRecordCommand>, CreateMedicalRecordCommandValidator>();
            services.AddScoped<IValidator<UpdateMedicalRecordCommand>, UpdateMedicalRecordCommandValidator>();
            return services;
        }
    }
}
