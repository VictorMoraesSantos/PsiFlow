using BuildingBlocks.CQRS.Sender;
using Patients.Application.DTOs.Patient;
using Patients.Application.Features.Patients.Commands.Create;
using Patients.Application.Features.Patients.Commands.Delete;
using Patients.Application.Features.Patients.Commands.Update;
using Patients.Application.Features.Patients.Queries.GetAll;
using Patients.Application.Features.Patients.Queries.GetById;
using System.Security.Claims;
using static BuildingBlocks.Authorization.Policies;

namespace Patients.API.Endpoints
{
    public static class PatientsEndpoints
    {
        public static IEndpointRouteBuilder MapPatientsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/v1/patients")
                .WithTags("Patients");

            group.MapGet("/", async (ISender sender, HttpContext http, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetPatientsQuery(http.GetTenantId(), http.GetUserId()), ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.Problem(result.Error!.Description, statusCode: StatusCodes.Status500InternalServerError);
            }).RequireAuthorization(Permissions.Patients.View);

            group.MapGet("/{id:int}", async (int id, ISender sender, HttpContext http, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetPatientByIdQuery(id, http.GetTenantId()), ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.NotFound(new { error = result.Error!.Description });
            }).RequireAuthorization(Permissions.Patients.View);

            group.MapPost("/", async (CreatePatientDTO dto, ISender sender, HttpContext http, CancellationToken ct) =>
            {
                var payload = dto with { TenantId = http.GetTenantId() };
                var result = await sender.Send(new CreatePatientCommand(payload, http.GetUserId()), ct);
                return result.IsSuccess
                    ? Results.Created($"/v1/patients/{result.Value!.Id}", result.Value)
                    : Results.BadRequest(new { error = result.Error!.Description });
            }).RequireAuthorization(Permissions.Patients.Create);

            group.MapPut("/{id:int}", async (int id, UpdatePatientDTO dto, ISender sender, HttpContext http, CancellationToken ct) =>
            {
                var payload = dto with { Id = id, TenantId = http.GetTenantId() };
                var result = await sender.Send(new UpdatePatientCommand(payload), ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error!.Description });
            }).RequireAuthorization(Permissions.Patients.Edit);

            group.MapDelete("/{id:int}", async (int id, ISender sender, HttpContext http, CancellationToken ct) =>
            {
                var result = await sender.Send(new DeletePatientCommand(id, http.GetTenantId(), http.GetUserId()), ct);
                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.NotFound(new { error = result.Error!.Description });
            }).RequireAuthorization(Permissions.Patients.Delete);

            return app;
        }
    }
}

internal static class PatientsHttpContextExtensions
{
    public static int GetTenantId(this HttpContext http) => int.TryParse(http.User.FindFirstValue("tenant_id"), out var id) ? id : 0;
    public static int GetUserId(this HttpContext http) => int.TryParse(http.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? http.User.FindFirstValue("sub"), out var id) ? id : 0;
}
