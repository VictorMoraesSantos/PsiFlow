using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Patients.Application.Features.Workflow;
using System.Security.Claims;
using static BuildingBlocks.Authorization.Policies;

namespace Patients.API.Endpoints;

public static class PatientInvitesEndpoints
{
    public static IEndpointRouteBuilder MapPatientInvitesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/v1/patients/{patientId:int}/deactivate", async (int patientId, DeactivatePatientRequest request, ISender sender, HttpContext http, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeactivatePatientCommand(patientId, request.Reason, http.GetTenantId(), http.GetUserId()), ct);
            return ToHttp(result, StatusCodes.Status204NoContent);
        }).RequireAuthorization(Permissions.Patients.Edit);

        app.MapPost("/v1/patients/{patientId:int}/status", async (int patientId, ChangeTreatmentStatusRequest request, ISender sender, HttpContext http, CancellationToken ct) =>
        {
            var result = await sender.Send(new ChangeTreatmentStatusCommand(patientId, request.TreatmentStatus, request.Reason, http.GetTenantId(), http.GetUserId()), ct);
            return ToHttp(result);
        }).RequireAuthorization(Permissions.Patients.Edit);

        app.MapGet("/v1/patients/{patientId:int}/sessions-summary", async (int patientId, ISender sender, CancellationToken ct) => ToHttp(await sender.Send(new GetPatientSessionsSummaryQuery(patientId), ct))).RequireAuthorization(Permissions.Patients.View);

        app.MapPost("/v1/patient-invites", async (InvitePatientRequest request, ISender sender, HttpContext http, CancellationToken ct) =>
        {
            var result = await sender.Send(new CreatePatientInviteCommand(request.Email, request.Phone, request.PatientId, http.GetTenantId(), http.GetUserId()), ct);
            return result.IsSuccess ? Results.Created("/v1/patient-invites", result.Value) : ToHttp(result);
        }).RequireAuthorization(Permissions.Patients.Create);

        app.MapGet("/v1/patient-invites/{token}/preview", async (string token, ISender sender, CancellationToken ct) => ToHttp(await sender.Send(new PreviewPatientInviteQuery(token), ct))).AllowAnonymous();

        app.MapPost("/v1/patient-invites/{token}/accept", async (string token, ISender sender, HttpContext http, CancellationToken ct) =>
        {
            var result = await sender.Send(new AcceptPatientInviteCommand(token, http.GetUserId(), http.GetEmail(), http.Connection.RemoteIpAddress?.ToString(), http.Request.Headers.UserAgent.ToString()), ct);
            return ToHttp(result);
        }).RequireAuthorization();

        app.MapPost("/v1/patient-invites/{inviteId:int}/revoke", async (int inviteId, ISender sender, HttpContext http, CancellationToken ct) =>
        {
            var result = await sender.Send(new RevokePatientInviteCommand(inviteId, http.GetTenantId()), ct);
            return ToHttp(result, StatusCodes.Status204NoContent);
        }).RequireAuthorization(Permissions.Patients.Delete);

        return app;
    }

    private static IResult ToHttp(Result result, int successStatus = StatusCodes.Status200OK) => result.IsSuccess
        ? successStatus == StatusCodes.Status204NoContent ? Results.NoContent() : Results.Ok()
        : result.Error!.Type switch
        {
            ErrorType.NotFound => Results.NotFound(new { error = result.Error.Description }),
            ErrorType.Conflict => Results.Conflict(new { error = result.Error.Description }),
            _ => Results.BadRequest(new { error = result.Error.Description })
        };

    private static IResult ToHttp<T>(Result<T> result) => result.IsSuccess
        ? Results.Ok(result.Value)
        : result.Error!.Type switch
        {
            ErrorType.NotFound => Results.NotFound(new { error = result.Error.Description }),
            ErrorType.Conflict => Results.Conflict(new { error = result.Error.Description }),
            _ => Results.BadRequest(new { error = result.Error.Description })
        };
}

public sealed record InvitePatientRequest(string Email, string? Phone, int? PatientId);
public sealed record DeactivatePatientRequest(string? Reason);
public sealed record ChangeTreatmentStatusRequest(string TreatmentStatus, string? Reason);

internal static class PatientHttpContextExtensions
{
    public static int GetTenantId(this HttpContext http) => int.TryParse(http.User.FindFirstValue("tenant_id"), out var id) ? id : 0;
    public static int GetUserId(this HttpContext http) => int.TryParse(http.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? http.User.FindFirstValue("sub"), out var id) ? id : 0;
    public static string GetEmail(this HttpContext http) => http.User.FindFirstValue(ClaimTypes.Email) ?? http.User.FindFirstValue("email") ?? string.Empty;
}
