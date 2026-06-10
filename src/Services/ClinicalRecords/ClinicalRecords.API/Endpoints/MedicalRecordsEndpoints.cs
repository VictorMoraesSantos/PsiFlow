using BuildingBlocks.CQRS.Sender;
using ClinicalRecords.Application.DTOs.MedicalRecord;
using ClinicalRecords.Application.Features.MedicalRecords.Commands.Create;
using ClinicalRecords.Application.Features.MedicalRecords.Commands.Delete;
using ClinicalRecords.Application.Features.MedicalRecords.Commands.Update;
using ClinicalRecords.Application.Features.MedicalRecords.Queries.GetAll;
using ClinicalRecords.Application.Features.MedicalRecords.Queries.GetById;

namespace ClinicalRecords.API.Endpoints
{
    public static class MedicalRecordsEndpoints
    {
        public static IEndpointRouteBuilder MapMedicalRecordsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/v1/clinical-records")
                .WithTags("MedicalRecords")
                .RequireAuthorization();

            group.MapGet("/", async (ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetMedicalRecordsQuery(), ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.Problem(result.Error!.Description, statusCode: StatusCodes.Status500InternalServerError);
            });

            group.MapGet("/{id:int}", async (int id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetMedicalRecordByIdQuery(id), ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.NotFound(new { error = result.Error!.Description });
            });

            group.MapPost("/", async (CreateMedicalRecordDTO dto, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new CreateMedicalRecordCommand(dto), ct);
                return result.IsSuccess
                    ? Results.Created($"/v1/clinical-records/{result.Value!.Id}", result.Value)
                    : Results.BadRequest(new { error = result.Error!.Description });
            });

            group.MapPut("/{id:int}", async (int id, UpdateMedicalRecordDTO dto, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new UpdateMedicalRecordCommand(id, dto), ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error!.Description });
            });

            group.MapDelete("/{id:int}", async (int id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new DeleteMedicalRecordCommand(id), ct);
                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.NotFound(new { error = result.Error!.Description });
            });

            return app;
        }
    }
}
