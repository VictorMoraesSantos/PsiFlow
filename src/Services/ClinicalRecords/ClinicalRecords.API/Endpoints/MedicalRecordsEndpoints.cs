using BuildingBlocks.CQRS.Sender;
using ClinicalRecords.Application.DTOs.MedicalRecord;
using ClinicalRecords.Application.Features.MedicalRecords.Commands.Create;
using ClinicalRecords.Application.Features.MedicalRecords.Commands.Delete;
using ClinicalRecords.Application.Features.MedicalRecords.Commands.Update;
using ClinicalRecords.Application.Features.MedicalRecords.Queries.GetAll;
using ClinicalRecords.Application.Features.MedicalRecords.Queries.GetById;
using static BuildingBlocks.Authorization.Policies;

namespace ClinicalRecords.API.Endpoints
{
    public static class MedicalRecordsEndpoints
    {
        public static IEndpointRouteBuilder MapMedicalRecordsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/v1/clinical-records")
                .WithTags("MedicalRecords");

            group.MapGet("/", () => Results.Forbid()).RequireAuthorization(Permissions.ClinicalRecords.View);

            group.MapGet("/{id:int}", () => Results.Forbid()).RequireAuthorization(Permissions.ClinicalRecords.View);

            group.MapPost("/", () => Results.Forbid()).RequireAuthorization(Permissions.ClinicalRecords.Create);

            group.MapPut("/{id:int}", () => Results.Forbid()).RequireAuthorization(Permissions.ClinicalRecords.Edit);

            group.MapDelete("/{id:int}", () => Results.Forbid()).RequireAuthorization(Permissions.ClinicalRecords.Delete);

            return app;
        }
    }
}
