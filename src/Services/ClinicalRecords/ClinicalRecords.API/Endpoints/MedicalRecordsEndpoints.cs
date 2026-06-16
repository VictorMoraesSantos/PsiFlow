using BuildingBlocks.CQRS.Sender;
using ClinicalRecords.Application.DTOs.MedicalRecord;
using ClinicalRecords.Application.Features.MedicalRecords.Commands.Create;
using ClinicalRecords.Application.Features.MedicalRecords.Commands.Delete;
using ClinicalRecords.Application.Features.MedicalRecords.Commands.Update;
using ClinicalRecords.Application.Features.MedicalRecords.Queries.GetAll;
using ClinicalRecords.Application.Features.MedicalRecords.Queries.GetById;
using System.Security.Claims;
using static BuildingBlocks.Authorization.Policies;

namespace ClinicalRecords.API.Endpoints
{
    public static class MedicalRecordsEndpoints
    {
        public static IEndpointRouteBuilder MapMedicalRecordsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/v1/clinical-records")
                .WithTags("MedicalRecords");

            group.MapGet("/", () => Results.Forbid())
                .RequireAuthorization(Permissions.ClinicalRecords.Read);

            group.MapGet("/{id:int}", () => Results.Forbid())
                .RequireAuthorization(Permissions.ClinicalRecords.Read);

            group.MapPost("/", () => Results.Forbid())
                .RequireAuthorization(Permissions.ClinicalRecords.Create);

            group.MapPut("/{id:int}", () => Results.Forbid())
                .RequireAuthorization(Permissions.ClinicalRecords.AnamnesisAutosave);

            group.MapDelete("/{id:int}", () => Results.Forbid())
                .RequireAuthorization(Permissions.ClinicalRecords.AuditRead);

            return app;
        }
    }
}
