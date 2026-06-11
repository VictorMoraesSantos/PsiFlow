using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PsiFlow.ClinicalRecords.Infrastructure.Persistence.Data;

public sealed class ClinicalRecordsDatabaseInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ClinicalRecordsDatabaseInitializer> _logger;

    public ClinicalRecordsDatabaseInitializer(IServiceProvider serviceProvider, ILogger<ClinicalRecordsDatabaseInitializer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ClinicalRecordsDbContext>();
        await context.Database.EnsureCreatedAsync(cancellationToken);
        _logger.LogInformation("Schema do banco de prontuarios verificado.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
