using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PsiFlow.OnlineSession.Infrastructure.Persistence;

public sealed class OnlineSessionDatabaseInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OnlineSessionDatabaseInitializer> _logger;

    public OnlineSessionDatabaseInitializer(IServiceProvider serviceProvider, ILogger<OnlineSessionDatabaseInitializer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OnlineSessionDbContext>();
        await context.Database.EnsureCreatedAsync(cancellationToken);
        _logger.LogInformation("Schema do banco de atendimento online verificado.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
