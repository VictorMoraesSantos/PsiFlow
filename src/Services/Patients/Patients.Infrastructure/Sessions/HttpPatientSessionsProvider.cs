using System.Net.Http.Json;
using BuildingBlocks.Results;
using Microsoft.Extensions.Configuration;
using Patients.Application.Contracts;

namespace Patients.Infrastructure.Sessions;

public sealed class HttpPatientSessionsProvider(HttpClient httpClient, IConfiguration configuration) : IPatientSessionsProvider
{
    public async Task<Result<IReadOnlyCollection<PatientSessionHistoryDTO>>> GetPatientSessionsAsync(int patientId, int tenantId, CancellationToken cancellationToken)
    {
        var baseUrl = configuration["Sessions:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl)) return Result.Success<IReadOnlyCollection<PatientSessionHistoryDTO>>(Array.Empty<PatientSessionHistoryDTO>());

        httpClient.BaseAddress ??= new Uri(baseUrl.TrimEnd('/') + "/");
        httpClient.DefaultRequestHeaders.Remove("X-Tenant-Id");
        httpClient.DefaultRequestHeaders.Add("X-Tenant-Id", tenantId.ToString());

        var sessions = await httpClient.GetFromJsonAsync<IReadOnlyCollection<PatientSessionHistoryDTO>>($"v1/patients/{patientId}/sessions", cancellationToken);
        return Result.Success(sessions ?? Array.Empty<PatientSessionHistoryDTO>());
    }
}
