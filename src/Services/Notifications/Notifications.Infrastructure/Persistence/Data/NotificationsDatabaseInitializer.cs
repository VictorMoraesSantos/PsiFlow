using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PsiFlow.Notifications.Infrastructure.Persistence.Data;

public sealed class NotificationsDatabaseInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationsDatabaseInitializer> _logger;

    public NotificationsDatabaseInitializer(IServiceProvider serviceProvider, ILogger<NotificationsDatabaseInitializer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        await context.Database.EnsureCreatedAsync(cancellationToken);
        _logger.LogInformation("Schema do banco de notificacoes verificado.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
