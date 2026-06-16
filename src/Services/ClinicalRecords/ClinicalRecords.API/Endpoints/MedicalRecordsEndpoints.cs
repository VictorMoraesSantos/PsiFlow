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
