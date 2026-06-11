using Auth.Domain.Entities;
using Auth.Domain.Enums;
using Auth.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

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
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            var canConnect = await context.Database.CanConnectAsync(cancellationToken);
            if (canConnect && !await HasTableAsync(context, "auth", "permission_groups", cancellationToken))
            {
                _logger.LogWarning("Dropping empty database before recreating schema.");
                await context.Database.EnsureDeletedAsync(cancellationToken);
                NpgsqlConnection.ClearAllPools();
            }
            await context.Database.EnsureCreatedAsync(cancellationToken);
            await ApplySchemaPatchesAsync(context, cancellationToken);
            await SeedRolesAsync(roleManager);

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

        private static async Task SeedRolesAsync(RoleManager<IdentityRole<int>> roleManager)
        {
            foreach (var role in new[] { "psychologist", "patient", "saas_admin" })
            {
                if (await roleManager.RoleExistsAsync(role)) continue;

                var result = await roleManager.CreateAsync(new IdentityRole<int>(role));
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(error => error.Description));
                    throw new InvalidOperationException($"Nao foi possivel criar a role '{role}': {errors}");
                }
            }
        }

        private static async Task ApplySchemaPatchesAsync(ApplicationDbContext context, CancellationToken cancellationToken)
        {
            await context.Database.ExecuteSqlRawAsync(
                "ALTER TABLE auth.users ADD COLUMN IF NOT EXISTS is_mfa_enabled boolean NOT NULL DEFAULT false;",
                cancellationToken);
        }

        private static async Task<bool> HasTableAsync(ApplicationDbContext context, string schema, string table, CancellationToken cancellationToken)
        {
            var conn = context.Database.GetDbConnection();
            var openedHere = false;
            if (conn.State != System.Data.ConnectionState.Open)
            {
                await conn.OpenAsync(cancellationToken);
                openedHere = true;
            }
            try
            {
                await using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = $1 AND table_name = $2)";
                var p1 = cmd.CreateParameter(); p1.Value = schema; cmd.Parameters.Add(p1);
                var p2 = cmd.CreateParameter(); p2.Value = table; cmd.Parameters.Add(p2);
                var result = await cmd.ExecuteScalarAsync(cancellationToken);
                return result is bool b && b;
            }
            finally
            {
                if (openedHere)
                {
                    await conn.CloseAsync();
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
