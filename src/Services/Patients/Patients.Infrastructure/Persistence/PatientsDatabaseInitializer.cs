using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PsiFlow.Patients.Infrastructure.Persistence;

public sealed class PatientsDatabaseInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PatientsDatabaseInitializer> _logger;

    public PatientsDatabaseInitializer(IServiceProvider serviceProvider, ILogger<PatientsDatabaseInitializer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PatientsDbContext>();

        await context.Database.EnsureCreatedAsync(cancellationToken);
        _logger.LogInformation("Schema do banco de pacientes verificado.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
