using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PsiFlow.Sessions.Infrastructure.Persistence;

public sealed class SessionsDatabaseInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SessionsDatabaseInitializer> _logger;

    public SessionsDatabaseInitializer(IServiceProvider serviceProvider, ILogger<SessionsDatabaseInitializer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SessionsDbContext>();
        await context.Database.EnsureCreatedAsync(cancellationToken);
        _logger.LogInformation("Schema do banco de sessoes verificado.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
