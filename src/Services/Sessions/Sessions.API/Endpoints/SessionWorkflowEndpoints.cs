using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Sessions.Application.Contracts;
using Sessions.Application.Features.Workflow;
using System.Security.Claims;
using static BuildingBlocks.Authorization.Policies;

namespace Sessions.API.Endpoints;

public static class SessionWorkflowEndpoints
{
    public static IEndpointRouteBuilder MapSessionWorkflowEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/v1/patients/{patientId:int}/sessions", async (int patientId, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
            ToHttp(await sender.Send(new GetPatientSessionsQuery(patientId, TenantId(user)), ct))).RequireAuthorization(Permissions.Sessions.PatientHistoryRead);

        app.MapPost("/v1/sessions/{sessionId:int}/start", async (int sessionId, StatusReasonRequest? request, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
            await ChangeStatus(sessionId, "in_progress", request, user, sender, ct)).RequireAuthorization(Permissions.Sessions.Start);

        app.MapPost("/v1/sessions/{sessionId:int}/complete", async (int sessionId, StatusReasonRequest? request, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
            await ChangeStatus(sessionId, "completed", request, user, sender, ct)).RequireAuthorization(Permissions.Sessions.Complete);

        app.MapPost("/v1/sessions/{sessionId:int}/no-show", async (int sessionId, StatusReasonRequest? request, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
            await ChangeStatus(sessionId, "no_show", request, user, sender, ct)).RequireAuthorization(Permissions.Sessions.NoShow);

        app.MapPost("/v1/sessions/{sessionId:int}/cancel", async (int sessionId, StatusReasonRequest? request, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
            await ChangeStatus(sessionId, "canceled", request, user, sender, ct)).RequireAuthorization(Permissions.Sessions.Cancel);

        app.MapPost("/v1/sessions/{sessionId:int}/payment/mark-received", async (int sessionId, MarkPaymentReceivedRequest request, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
            ToHttp(await sender.Send(new MarkPaymentReceivedCommand(sessionId, request, TenantId(user), UserId(user)), ct))).RequireAuthorization(Permissions.Sessions.PaymentMarkReceived);

        app.MapGet("/v1/sessions/{sessionId:int}/payment", async (int sessionId, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
            ToHttp(await sender.Send(new GetSessionPaymentQuery(sessionId, TenantId(user)), ct))).RequireAuthorization(Permissions.Sessions.PaymentRead);

        app.MapPost("/v1/sessions/{sessionId:int}/receipt/send", async (int sessionId, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
            ToHttp(await sender.Send(new SendReceiptCommand(sessionId, TenantId(user)), ct))).RequireAuthorization(Permissions.Sessions.ReceiptSend);

        return app;
    }

    private static async Task<IResult> ChangeStatus(int sessionId, string status, StatusReasonRequest? request, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
        ToHttp(await sender.Send(new ChangeSessionStatusCommand(sessionId, new ChangeSessionStatusRequest(status, request?.Reason), TenantId(user), UserId(user)), ct));

    private static IResult ToHttp<T>(Result<T> result)
    {
        if (result.IsSuccess) return Results.Ok(result.Value);
        return result.Error?.Type switch
        {
            ErrorType.NotFound => Results.NotFound(new { error = result.Error.Description }),
            ErrorType.Conflict => Results.Conflict(new { error = result.Error.Description }),
            ErrorType.Forbidden => Results.Forbid(),
            _ => Results.BadRequest(new { error = result.Error?.Description })
        };
    }

    private static int TenantId(ClaimsPrincipal user) => ReadIntClaim(user, "tenant_id");
    private static int UserId(ClaimsPrincipal user) => ReadIntClaim(user, ClaimTypes.NameIdentifier, "sub", "user_id");

    private static int ReadIntClaim(ClaimsPrincipal user, params string[] names) =>
        names.Select(name => user.FindFirstValue(name)).Select(value => int.TryParse(value, out var id) ? id : 0).FirstOrDefault(id => id > 0);

    private sealed record StatusReasonRequest(string? Reason);
}
