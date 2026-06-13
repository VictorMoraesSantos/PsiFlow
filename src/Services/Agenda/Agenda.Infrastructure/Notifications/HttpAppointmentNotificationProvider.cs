using System.Net.Http.Json;
using System.Text.Json;
using Agenda.Application.Contracts;
using BuildingBlocks.Results;
using Microsoft.Extensions.Configuration;

namespace Agenda.Infrastructure.Notifications;

public sealed class HttpAppointmentNotificationProvider(HttpClient httpClient, IConfiguration configuration) : IAppointmentNotificationProvider
{
    public async Task<Result> SendAppointmentScheduledAsync(AppointmentScheduledNotification notification, CancellationToken cancellationToken)
    {
        var baseUrl = configuration["Notifications:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl)) return Result.Success();

        httpClient.BaseAddress ??= new Uri(baseUrl.TrimEnd('/') + "/");
        var body = new
        {
            notificationType = "appointment_confirmation",
            scheduledFor = DateTime.UtcNow,
            recipientEmail = (string?)null,
            recipientUserId = (int?)null,
            payloadJson = JsonSerializer.Serialize(notification)
        };

        var response = await httpClient.PostAsJsonAsync("v1/notifications/schedule-reminders", body, cancellationToken);
        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(Error.Failure($"Failed to schedule appointment confirmation: {(int)response.StatusCode}"));
    }
}
