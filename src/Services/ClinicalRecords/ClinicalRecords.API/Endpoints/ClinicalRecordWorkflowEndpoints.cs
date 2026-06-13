using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using ClinicalRecords.Application.Features.Workflow;
using System.Security.Claims;
using static BuildingBlocks.Authorization.Policies;

namespace ClinicalRecords.API.Endpoints;

public static class ClinicalRecordWorkflowEndpoints
{
    public static IEndpointRouteBuilder MapClinicalRecordWorkflowEndpoints(this IEndpointRouteBuilder app)
    {
        var psych = app.MapGroup("/v1").WithTags("ClinicalRecords");
        psych.MapGet("/patients/{patientId:int}/clinical-record", async (int patientId, ISender sender, HttpContext http, CancellationToken ct) => RejectClinicalContentAccess(http) ?? ToHttp(await sender.Send(new GetClinicalRecordByPatientQuery(patientId, http.GetTenantId()), ct))).RequireAuthorization(Permissions.ClinicalRecords.View);
        psych.MapPost("/patients/{patientId:int}/clinical-record", async (int patientId, ClinicalRecordBody body, ISender sender, HttpContext http, CancellationToken ct) => RejectClinicalContentAccess(http) ?? ToHttp(await sender.Send(new CreateClinicalRecordCommand(patientId, http.GetTenantId(), body.Name), ct))).RequireAuthorization(Permissions.ClinicalRecords.Create);
        psych.MapGet("/clinical-records/{recordId:int}", async (int recordId, ISender sender, HttpContext http, CancellationToken ct) => RejectClinicalContentAccess(http) ?? ToHttp(await sender.Send(new GetClinicalRecordQuery(recordId, http.GetTenantId()), ct))).RequireAuthorization(Permissions.ClinicalRecords.View);
        psych.MapGet("/clinical-records/{recordId:int}/anamnesis", async (int recordId, ISender sender, HttpContext http, CancellationToken ct) => RejectClinicalContentAccess(http) ?? ToHttp(await sender.Send(new GetAnamnesisQuery(recordId, http.GetTenantId()), ct))).RequireAuthorization(Permissions.ClinicalRecords.View);
        psych.MapPost("/clinical-records/{recordId:int}/anamnesis/autosave", async (int recordId, EncryptedDraftBody body, ISender sender, HttpContext http, CancellationToken ct) => RejectClinicalContentAccess(http) ?? ToHttp(await sender.Send(new AutosaveAnamnesisCommand(recordId, http.GetTenantId(), body.Ciphertext, body.Nonce, body.Tag), ct), StatusCodes.Status204NoContent)).RequireAuthorization(Permissions.ClinicalRecords.Edit);
        psych.MapPost("/clinical-records/{recordId:int}/anamnesis/publish-version", async (int recordId, PublishVersionBody body, ISender sender, HttpContext http, CancellationToken ct) => RejectClinicalContentAccess(http) ?? ToHttp(await sender.Send(new PublishAnamnesisVersionCommand(recordId, http.GetTenantId(), http.GetUserId(), body.Reason), ct))).RequireAuthorization(Permissions.ClinicalRecords.Edit);
        psych.MapGet("/sessions/{sessionId:int}/evolution", async (int sessionId, ISender sender, HttpContext http, CancellationToken ct) => RejectClinicalContentAccess(http) ?? ToHttp(await sender.Send(new GetEvolutionQuery(sessionId, http.GetTenantId()), ct))).RequireAuthorization(Permissions.ClinicalRecords.View);
        psych.MapPost("/sessions/{sessionId:int}/evolution/autosave", async (int sessionId, EncryptedDraftBody body, ISender sender, HttpContext http, CancellationToken ct) => RejectClinicalContentAccess(http) ?? ToHttp(await sender.Send(new AutosaveEvolutionCommand(sessionId, http.GetTenantId(), body.Ciphertext, body.Nonce, body.Tag), ct), StatusCodes.Status204NoContent)).RequireAuthorization(Permissions.ClinicalRecords.Edit);
        psych.MapPost("/sessions/{sessionId:int}/evolution/publish-version", async (int sessionId, PublishVersionBody body, ISender sender, HttpContext http, CancellationToken ct) => RejectClinicalContentAccess(http) ?? ToHttp(await sender.Send(new PublishEvolutionVersionCommand(sessionId, http.GetTenantId(), http.GetUserId(), body.Reason), ct))).RequireAuthorization(Permissions.ClinicalRecords.Edit);
        psych.MapGet("/sessions/{sessionId:int}/evolution/versions", async (int sessionId, ISender sender, HttpContext http, CancellationToken ct) => RejectClinicalContentAccess(http) ?? ToHttp(await sender.Send(new GetEvolutionVersionsQuery(sessionId, http.GetTenantId()), ct))).RequireAuthorization(Permissions.ClinicalRecords.View);
        psych.MapGet("/clinical-records/{recordId:int}/audit-log", async (int recordId, ISender sender, HttpContext http, CancellationToken ct) => RejectClinicalContentAccess(http) ?? ToHttp(await sender.Send(new GetClinicalAuditLogQuery(recordId, http.GetTenantId()), ct))).RequireAuthorization(Permissions.ClinicalRecords.View);
        return app;
    }

    private static IResult? RejectClinicalContentAccess(HttpContext http)
    {
        if (http.User.IsInRole("patient")) return Results.Forbid();
        if (http.User.IsInRole("saas_admin")) return Results.Forbid();
        return null;
    }

    private static IResult ToHttp(Result result, int successStatus = StatusCodes.Status200OK) => result.IsSuccess ? successStatus == StatusCodes.Status204NoContent ? Results.NoContent() : Results.Ok() : Results.BadRequest(new { error = result.Error!.Description });
    private static IResult ToHttp<T>(Result<T> result) => result.IsSuccess ? Results.Ok(result.Value) : result.Error!.Type == ErrorType.NotFound ? Results.NotFound(new { error = result.Error.Description }) : Results.BadRequest(new { error = result.Error.Description });
}

internal static class ClinicalRecordHttpContextExtensions
{
    public static int GetTenantId(this HttpContext http) => int.TryParse(http.User.FindFirstValue("tenant_id"), out var id) ? id : 0;
    public static int GetUserId(this HttpContext http) => int.TryParse(http.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? http.User.FindFirstValue("sub"), out var id) ? id : 0;
}
