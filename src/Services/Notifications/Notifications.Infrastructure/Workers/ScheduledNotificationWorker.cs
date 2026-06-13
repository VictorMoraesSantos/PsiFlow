using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Notifications.Application.Email;
using Notifications.Domain.Repositories;

namespace Notifications.Infrastructure.Workers
{
    public sealed class ScheduledNotificationWorker(
        ILogger<ScheduledNotificationWorker> logger,
        IServiceScopeFactory scopeFactory) : BackgroundService
    {
        private const int MaxAttempts = 5;
        private const int MaxBatch = 50;
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(30);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessBatchAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Falha ao processar ScheduledNotifications");
                }

                try
                {
                    await Task.Delay(PollInterval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        private async Task ProcessBatchAsync(CancellationToken cancellationToken)
        {
            using var scope = scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IScheduledNotificationRepository>();
            var provider = scope.ServiceProvider.GetRequiredService<IEmailProvider>();

            var due = await repository.ClaimDueAsync(DateTime.UtcNow, MaxBatch, MaxAttempts, cancellationToken);
            foreach (var item in due)
            {
                try
                {
                    var result = await provider.SendAsync(
                        new EmailMessage(item.RecipientEmail, item.NotificationType, item.PayloadJson, item.PayloadJson),
                        cancellationToken);
                    if (result.IsSuccess)
                    {
                        item.Status = "sent";
                        item.LastError = null;
                    }
                    else
                    {
                        item.AttemptCount++;
                        item.LastError = result.Error!.Description;
                        item.Status = item.AttemptCount >= MaxAttempts ? "failed" : "pending";
                    }
                }
                catch (Exception ex)
                {
                    item.AttemptCount++;
                    item.LastError = ex.Message;
                    item.Status = item.AttemptCount >= MaxAttempts ? "failed" : "pending";
                    logger.LogWarning(ex, "Falha ao enviar scheduled notification {Id}", item.Id);
                }
                await repository.Update(item, cancellationToken);
            }
        }
    }
}
