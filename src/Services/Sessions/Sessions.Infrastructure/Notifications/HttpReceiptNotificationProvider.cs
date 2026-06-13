using System.Net.Http.Json;
using System.Text.Json;
using BuildingBlocks.Results;
using Microsoft.Extensions.Configuration;
using Sessions.Application.Contracts;

namespace Sessions.Infrastructure.Notifications;

public sealed class HttpReceiptNotificationProvider(HttpClient httpClient, IConfiguration configuration) : IReceiptNotificationProvider
{
    public async Task<Result<int?>> SendReceiptAsync(ReceiptNotificationRequest request, CancellationToken cancellationToken)
    {
        var baseUrl = configuration["Notifications:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl)) return Result.Success<int?>(null);

        httpClient.BaseAddress ??= new Uri(baseUrl.TrimEnd('/') + "/");
        var body = new
        {
            notificationType = "payment_receipt",
            scheduledFor = DateTime.UtcNow,
            recipientEmail = (string?)null,
            recipientUserId = (int?)null,
            payloadJson = JsonSerializer.Serialize(request)
        };

        var response = await httpClient.PostAsJsonAsync("v1/notifications/schedule-reminders", body, cancellationToken);
        if (!response.IsSuccessStatusCode) return Result.Failure<int?>(Error.Failure($"Failed to schedule receipt notification: {(int)response.StatusCode}"));

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);
        return json.TryGetProperty("id", out var id) && id.TryGetInt32(out var value)
            ? Result.Success<int?>(value)
            : Result.Success<int?>(null);
    }
}
