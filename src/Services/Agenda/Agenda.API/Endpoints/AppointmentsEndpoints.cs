using Agenda.Application.DTOs.Appointment;
using Agenda.Application.Features.Appointments.Commands.Create;
using Agenda.Application.Features.Appointments.Commands.Delete;
using Agenda.Application.Features.Appointments.Commands.Update;
using Agenda.Application.Features.Appointments.Queries.GetAll;
using Agenda.Application.Features.Appointments.Queries.GetById;
using BuildingBlocks.Authorization;
using BuildingBlocks.CQRS.Sender;
using static BuildingBlocks.Authorization.Policies;

namespace Agenda.API.Endpoints
{
    public static class AppointmentsEndpoints
    {
        public static IEndpointRouteBuilder MapAppointmentsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/v1/appointments")
                .WithTags("Appointments");

            group.MapGet("/", async (ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetAppointmentsQuery(), ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.Problem(result.Error!.Description, statusCode: StatusCodes.Status500InternalServerError);
            }).RequireAuthorization(Permissions.Agenda.View);

            group.MapGet("/{id:int}", async (int id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetAppointmentByIdQuery(id), ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.NotFound(new { error = result.Error!.Description });
            }).RequireAuthorization(Permissions.Agenda.View);

            group.MapPost("/", async (CreateAppointmentDTO dto, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new CreateAppointmentCommand(dto), ct);
                return result.IsSuccess
                    ? Results.Created($"/v1/appointments/{result.Value!.Id}", result.Value)
                    : Results.BadRequest(new { error = result.Error!.Description });
            }).RequireAuthorization(Permissions.Agenda.Create);

            group.MapPut("/{id:int}", async (int id, UpdateAppointmentDTO dto, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new UpdateAppointmentCommand(id, dto), ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error!.Description });
            }).RequireAuthorization(Permissions.Agenda.Edit);

            group.MapDelete("/{id:int}", async (int id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new DeleteAppointmentCommand(id), ct);
                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.NotFound(new { error = result.Error!.Description });
            }).RequireAuthorization(Permissions.Agenda.Delete);

            return app;
        }
    }
}
