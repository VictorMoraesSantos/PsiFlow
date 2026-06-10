namespace PsiFlow.ClinicalRecords.API.Endpoints;

public static class ClinicalRecordsEndpoints
{
    public static IEndpointRouteBuilder MapClinicalRecordsEndpoints(this IEndpointRouteBuilder app)
    {
        var psych = app.MapGroup("/v1").WithTags("ClinicalRecords").RequireAuthorization("RequirePsychologist");

        psych.MapGet("/patients/{patientId:guid}/clinical-record", (Guid patientId) => Results.Ok(new { patientId }));
        psych.MapPost("/patients/{patientId:guid}/clinical-record", (Guid patientId) => Results.Created($"/v1/clinical-records/{Guid.NewGuid()}", new { patientId }));
        psych.MapGet("/clinical-records/{recordId:guid}", (Guid id) => Results.Ok(new { id }));
        psych.MapGet("/clinical-records/{recordId:guid}/anamnesis", (Guid id) => Results.Ok(new { id }));
        psych.MapPost("/clinical-records/{recordId:guid}/anamnesis/autosave", (Guid id) => Results.NoContent());
        psych.MapPost("/clinical-records/{recordId:guid}/anamnesis/publish-version", (Guid id) => Results.NoContent());
        psych.MapGet("/sessions/{sessionId:guid}/evolution", (Guid id) => Results.Ok(new { id }));
        psych.MapPost("/sessions/{sessionId:guid}/evolution/autosave", (Guid id) => Results.NoContent());
        psych.MapPost("/sessions/{sessionId:guid}/evolution/publish-version", (Guid id) => Results.NoContent());
        psych.MapGet("/sessions/{sessionId:guid}/evolution/versions", (Guid id) => Results.Ok(Array.Empty<object>()));
        psych.MapGet("/clinical-records/{recordId:guid}/audit-log", (Guid id) => Results.Ok(Array.Empty<object>()));
        return app;
    }
}
