using System.Net.Http.Json;
using Agenda.Application.Contracts;
using BuildingBlocks.Results;
using Microsoft.Extensions.Configuration;

namespace Agenda.Infrastructure.Sessions;

public sealed class HttpAppointmentSessionProvider(HttpClient httpClient, IConfiguration configuration) : IAppointmentSessionProvider
{
    public async Task<Result> CreateSessionForAppointmentAsync(AppointmentSessionRequest request, CancellationToken cancellationToken)
    {
        var baseUrl = configuration["Sessions:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl)) return Result.Success();

        httpClient.BaseAddress ??= new Uri(baseUrl.TrimEnd('/') + "/");
        var body = new
        {
            tenantId = request.TenantId,
            name = request.Name,
            appointmentId = request.AppointmentId,
            patientId = request.PatientId,
            psychologistId = request.PsychologistId,
            startsAt = request.StartsAt,
            endsAt = request.EndsAt,
            status = "scheduled",
            modality = request.Modality
        };

        var response = await httpClient.PostAsJsonAsync("v1/sessions/from-appointment", body, cancellationToken);
        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(Error.Failure($"Failed to create session for appointment: {(int)response.StatusCode}"));
    }

    public async Task<Result> CancelSessionForAppointmentAsync(int tenantId, int appointmentId, string? reason, CancellationToken cancellationToken)
    {
        var baseUrl = configuration["Sessions:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl)) return Result.Success();

        httpClient.BaseAddress ??= new Uri(baseUrl.TrimEnd('/') + "/");
        var response = await httpClient.PostAsJsonAsync($"v1/sessions/by-appointment/{appointmentId}/cancel", new { tenantId, reason }, cancellationToken);
        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(Error.Failure($"Failed to cancel session for appointment: {(int)response.StatusCode}"));
    }
}
