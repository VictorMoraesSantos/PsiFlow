using BuildingBlocks.CQRS.Sender;
using Patients.Application.Contracts;
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
            }).RequireAuthorization(Permissions.Patients.List);

            group.MapGet("/{id:int}", async (int id, ISender sender, HttpContext http, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetPatientByIdQuery(id, http.GetTenantId()), ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : ToError(result.Error!);
            }).RequireAuthorization(Permissions.Patients.Read);

            group.MapGet("/{id:int}/administrative-profile", async (int id, IPatientService service, HttpContext http, CancellationToken ct) =>
            {
                var result = await service.GetAdministrativeProfileAsync(id, http.GetTenantId(), ct);
                return result.IsSuccess ? Results.Ok(result.Value) : ToError(result.Error!);
            }).RequireAuthorization(Permissions.Patients.Read);

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
                    : ToError(result.Error!);
            }).RequireAuthorization(Permissions.Patients.Update);

            group.MapPatch("/{id:int}/administrative-profile", async (int id, PatchPatientAdministrativeDTO dto, IPatientService service, HttpContext http, CancellationToken ct) =>
            {
                var result = await service.PatchAdministrativeProfileAsync(id, http.GetTenantId(), dto, ct);
                return result.IsSuccess ? Results.Ok(result.Value) : ToError(result.Error!);
            }).RequireAuthorization(Permissions.Patients.Update);

            group.MapPost("/{id:int}/administrative-notes", async (int id, CreatePatientAdministrativeNoteDTO dto, IPatientService service, HttpContext http, CancellationToken ct) =>
            {
                var result = await service.AddAdministrativeNoteAsync(id, http.GetTenantId(), http.GetUserId(), dto, ct);
                return result.IsSuccess ? Results.Created($"/v1/patients/{id}/administrative-notes/{result.Value!.Id}", result.Value) : ToError(result.Error!);
            }).RequireAuthorization(Permissions.Patients.AdminNotesManage);

            group.MapPut("/{id:int}/emergency-contact", async (int id, UpsertEmergencyContactDTO dto, IPatientService service, HttpContext http, CancellationToken ct) =>
            {
                var result = await service.UpsertEmergencyContactAsync(id, http.GetTenantId(), dto, ct);
                return result.IsSuccess ? Results.Ok(result.Value) : ToError(result.Error!);
            }).RequireAuthorization(Permissions.Patients.EmergencyContactManage);

            group.MapPut("/{id:int}/legal-responsible", async (int id, UpsertResponsibleLegalDTO dto, IPatientService service, HttpContext http, CancellationToken ct) =>
            {
                var result = await service.UpsertResponsibleLegalAsync(id, http.GetTenantId(), dto, ct);
                return result.IsSuccess ? Results.Ok(result.Value) : ToError(result.Error!);
            }).RequireAuthorization(Permissions.Patients.LegalResponsibleManage);

            group.MapGet("/{id:int}/legal-responsible", async (int id, IPatientService service, HttpContext http, CancellationToken ct) =>
            {
                var result = await service.GetResponsibleLegalAsync(id, http.GetTenantId(), ct);
                return result.IsSuccess
                    ? result.Value is null ? Results.NotFound() : Results.Ok(result.Value)
                    : ToError(result.Error!);
            }).RequireAuthorization(Permissions.Patients.LegalResponsibleManage);

            group.MapDelete("/{id:int}", async (int id, ISender sender, HttpContext http, CancellationToken ct) =>
            {
                var result = await sender.Send(new DeletePatientCommand(id, http.GetTenantId(), http.GetUserId()), ct);
                return result.IsSuccess
                    ? Results.NoContent()
                    : ToError(result.Error!);
            }).RequireAuthorization(Permissions.Patients.Deactivate);

            return app;
        }

        private static IResult ToError(BuildingBlocks.Results.Error error) => error.Type switch
        {
            BuildingBlocks.Results.ErrorType.NotFound => Results.NotFound(new { error = error.Description }),
            BuildingBlocks.Results.ErrorType.Forbidden => Results.Forbid(),
            BuildingBlocks.Results.ErrorType.Conflict => Results.Conflict(new { error = error.Description }),
            _ => Results.BadRequest(new { error = error.Description })
        };
    }
}

internal static class PatientsHttpContextExtensions
{
    public static int GetTenantId(this HttpContext http) => int.TryParse(http.User.FindFirstValue("tenant_id"), out var id) ? id : 0;
    public static int GetUserId(this HttpContext http) => int.TryParse(http.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? http.User.FindFirstValue("sub"), out var id) ? id : 0;
}
