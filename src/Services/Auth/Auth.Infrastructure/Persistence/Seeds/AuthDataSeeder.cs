using Auth.Domain.Aggregates;
using Auth.Domain.Enums;
using Auth.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Auth.Infrastructure.Persistence.Seeds
{
    public class AuthDataSeeder : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AuthDataSeeder> _logger;

        public AuthDataSeeder(IServiceProvider serviceProvider, ILogger<AuthDataSeeder> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync(cancellationToken);

            if (!await context.PermissionGroups.AnyAsync(cancellationToken))
            {
                foreach (var group in PermissionGroupSeed.DefaultGroups())
                {
                    await context.PermissionGroups.AddAsync(group, cancellationToken);
                }
                await context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Seed de grupos de permissao aplicado.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
