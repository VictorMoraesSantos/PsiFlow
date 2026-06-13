using Agenda.Application.Contracts;
using Agenda.Application.Features.Workflow;
using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using System.Security.Claims;
using static BuildingBlocks.Authorization.Policies;

namespace Agenda.API.Endpoints;

public static class AgendaEndpoints
{
    public static IEndpointRouteBuilder MapAgendaEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/v1/availability/weekly", async (WeeklyAvailabilityRequest request, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
            ToHttp(await sender.Send(new CreateWeeklyAvailabilityCommand(request, TenantId(user)), ct), value => Results.Created($"/v1/availability/weekly/{value.Id}", value))).RequireAuthorization(Permissions.Agenda.Create);

        app.MapGet("/v1/availability/weekly", async (ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
            ToHttp(await sender.Send(new GetWeeklyAvailabilitiesQuery(TenantId(user)), ct))).RequireAuthorization(Permissions.Agenda.View);

        app.MapPatch("/v1/availability/weekly/{availabilityId:int}", async (int availabilityId, WeeklyAvailabilityRequest request, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
            ToHttp(await sender.Send(new UpdateWeeklyAvailabilityCommand(availabilityId, request, TenantId(user)), ct))).RequireAuthorization(Permissions.Agenda.Edit);

        app.MapDelete("/v1/availability/weekly/{availabilityId:int}", async (int availabilityId, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
            ToHttp(await sender.Send(new DeleteWeeklyAvailabilityCommand(availabilityId, TenantId(user)), ct), _ => Results.NoContent())).RequireAuthorization(Permissions.Agenda.Delete);

        app.MapPost("/v1/schedule-blocks", async (ScheduleBlockRequest request, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
            ToHttp(await sender.Send(new CreateScheduleBlockCommand(request, TenantId(user), UserId(user)), ct), value => Results.Created($"/v1/schedule-blocks/{value.Id}", value))).RequireAuthorization(Permissions.Agenda.Create);

        app.MapGet("/v1/schedule-blocks", async (ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
            ToHttp(await sender.Send(new GetScheduleBlocksQuery(TenantId(user)), ct))).RequireAuthorization(Permissions.Agenda.View);

        app.MapDelete("/v1/schedule-blocks/{blockId:int}", async (int blockId, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
            ToHttp(await sender.Send(new DeleteScheduleBlockCommand(blockId, TenantId(user)), ct), _ => Results.NoContent())).RequireAuthorization(Permissions.Agenda.Delete);

        app.MapGet("/v1/available-slots", async (DateTime from, DateTime to, string? modality, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
            ToHttp(await sender.Send(new GetAvailableSlotsQuery(new AvailableSlotsRequest(from, to, modality), TenantId(user)), ct))).RequireAuthorization(Permissions.Agenda.View);

        app.MapPost("/v1/appointments/{appointmentId:int}/cancel", async (int appointmentId, CancelAppointmentRequest request, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
            ToHttp(await sender.Send(new CancelAppointmentCommand(appointmentId, request, TenantId(user), UserId(user)), ct))).RequireAuthorization(Permissions.Agenda.Edit);

        return app;
    }

    private static IResult ToHttp<T>(Result<T> result, Func<T, IResult>? onSuccess = null)
    {
        if (result.IsSuccess) return onSuccess is null ? Results.Ok(result.Value) : onSuccess(result.Value!);
        return result.Error?.Type == ErrorType.NotFound
            ? Results.NotFound(new { error = result.Error.Description })
            : Results.BadRequest(new { error = result.Error?.Description });
    }

    private static int TenantId(ClaimsPrincipal user) => ReadIntClaim(user, "tenant_id");
    private static int UserId(ClaimsPrincipal user) => ReadIntClaim(user, ClaimTypes.NameIdentifier, "sub", "user_id");

    private static int ReadIntClaim(ClaimsPrincipal user, params string[] names) =>
        names.Select(name => user.FindFirstValue(name)).Select(value => int.TryParse(value, out var id) ? id : 0).FirstOrDefault(id => id > 0);
}
