using Auth.Application.Authorization;
using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
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
        public const string WildcardPermissionClaim = "*";

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
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<UserId>>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var canConnect = await context.Database.CanConnectAsync(cancellationToken);
            if (canConnect &&
                (!await HasTableAsync(context, "auth", "permission_groups", cancellationToken) ||
                 !await HasTableAsync(context, "public", "AspNetRoles", cancellationToken)))
            {
                _logger.LogWarning("Dropping empty database before recreating schema.");
                await context.Database.EnsureDeletedAsync(cancellationToken);
                NpgsqlConnection.ClearAllPools();
            }
            await context.Database.EnsureCreatedAsync(cancellationToken);
            await ApplySchemaPatchesAsync(context, cancellationToken);
            await SeedRolesAsync(roleManager);
            await SeedPermissionGroupsAsync(context, cancellationToken);
            await SeedUsersAsync(userManager, context, cancellationToken);
        }

        private async Task SeedPermissionGroupsAsync(ApplicationDbContext context, CancellationToken cancellationToken)
        {
            if (await context.PermissionGroups.AnyAsync(cancellationToken)) return;

            foreach (var group in PermissionGroupSeed.DefaultGroups())
            {
                await context.PermissionGroups.AddAsync(group, cancellationToken);
            }
            await context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Seed de grupos de permissao aplicado (6 grupos, 24 permissoes).");
        }

        private async Task SeedUsersAsync(UserManager<User> userManager, ApplicationDbContext context, CancellationToken cancellationToken)
        {
            await SeedAdminUserAsync(userManager, context, cancellationToken);
            await SeedPsychologistUserAsync(userManager, context, cancellationToken);
            await SeedPatientUserAsync(userManager, context, cancellationToken);
        }

        private async Task SeedAdminUserAsync(UserManager<User> userManager, ApplicationDbContext context, CancellationToken cancellationToken)
        {
            var existing = await userManager.FindByEmailAsync(PermissionGroupSeed.AdminEmail);
            if (existing is not null)
            {
                if (!await userManager.IsInRoleAsync(existing, PermissionGroupSeed.AdminRole))
                    await userManager.AddToRoleAsync(existing, PermissionGroupSeed.AdminRole);
                await AssignAllPermissionClaimsAsync(userManager, existing, context, cancellationToken);
                return;
            }

            var name = new Name("Admin", "PsiFlow");
            var contact = new Contact(PermissionGroupSeed.AdminEmail, "+5511999999999");
            var user = new User(
                name,
                contact,
                PermissionGroupSeed.AdminRole,
                tenantId: null,
                crp: null,
                termsVersion: "v1",
                privacyVersion: "v1");

            var result = await userManager.CreateAsync(user, PermissionGroupSeed.AdminPassword);
            if (!result.Succeeded)
            {
                _logger.LogError("Falha ao criar admin user: {Errors}", string.Join("; ", result.Errors.Select(e => e.Description)));
                return;
            }

            await userManager.AddToRoleAsync(user, PermissionGroupSeed.AdminRole);
            await AssignAllPermissionClaimsAsync(userManager, user, context, cancellationToken);
            _logger.LogInformation("Admin user criado: {Email}", PermissionGroupSeed.AdminEmail);
        }

        private async Task SeedPsychologistUserAsync(UserManager<User> userManager, ApplicationDbContext context, CancellationToken cancellationToken)
        {
            var existing = await userManager.FindByEmailAsync(PermissionGroupSeed.PsychologistEmail);
            if (existing is not null)
            {
                if (!await userManager.IsInRoleAsync(existing, PermissionGroupSeed.PsychologistRole))
                    await userManager.AddToRoleAsync(existing, PermissionGroupSeed.PsychologistRole);
                await AssignGroupPermissionClaimsAsync(userManager, existing, "patients", "sessions", "agenda", "clinical_records", "online_session");
                return;
            }

            var name = new Name("Psicologa", "Demo");
            var contact = new Contact(PermissionGroupSeed.PsychologistEmail, "+5511988887777");
            var user = new User(
                name,
                contact,
                PermissionGroupSeed.PsychologistRole,
                tenantId: null,
                crp: "06/123456",
                termsVersion: "v1",
                privacyVersion: "v1");

            var result = await userManager.CreateAsync(user, PermissionGroupSeed.PsychologistPassword);
            if (!result.Succeeded)
            {
                _logger.LogError("Falha ao criar psychologist user: {Errors}", string.Join("; ", result.Errors.Select(e => e.Description)));
                return;
            }

            await userManager.AddToRoleAsync(user, PermissionGroupSeed.PsychologistRole);
            await AssignGroupPermissionClaimsAsync(userManager, user, "patients", "sessions", "agenda", "clinical_records", "online_session");
            _logger.LogInformation("Psychologist user criado: {Email}", PermissionGroupSeed.PsychologistEmail);
        }

        private async Task SeedPatientUserAsync(UserManager<User> userManager, ApplicationDbContext context, CancellationToken cancellationToken)
        {
            var existing = await userManager.FindByEmailAsync(PermissionGroupSeed.PatientEmail);
            if (existing is not null)
            {
                if (!await userManager.IsInRoleAsync(existing, PermissionGroupSeed.PatientRole))
                    await userManager.AddToRoleAsync(existing, PermissionGroupSeed.PatientRole);
                await AssignGroupPermissionClaimsAsync(userManager, existing, "patients", "sessions", "agenda", "online_session");
                return;
            }

            var name = new Name("Paciente", "Demo");
            var contact = new Contact(PermissionGroupSeed.PatientEmail, "+5511977776666");
            var user = new User(
                name,
                contact,
                PermissionGroupSeed.PatientRole,
                tenantId: null,
                crp: null,
                termsVersion: "v1",
                privacyVersion: "v1");

            var result = await userManager.CreateAsync(user, PermissionGroupSeed.PatientPassword);
            if (!result.Succeeded)
            {
                _logger.LogError("Falha ao criar patient user: {Errors}", string.Join("; ", result.Errors.Select(e => e.Description)));
                return;
            }

            await userManager.AddToRoleAsync(user, PermissionGroupSeed.PatientRole);
            await AssignGroupPermissionClaimsAsync(userManager, user, "patients", "sessions", "agenda", "online_session");
            _logger.LogInformation("Patient user criado: {Email}", PermissionGroupSeed.PatientEmail);
        }

        private async Task AssignAllPermissionClaimsAsync(UserManager<User> userManager, User user, ApplicationDbContext context, CancellationToken cancellationToken)
        {
            var existingClaims = await userManager.GetClaimsAsync(user);
            foreach (var claim in existingClaims.Where(c => c.Type == "permission").ToList())
            {
                await userManager.RemoveClaimAsync(user, claim);
            }

            await userManager.AddClaimAsync(user, new System.Security.Claims.Claim("permission", WildcardPermissionClaim));

            foreach (var permission in PermissionCatalog.SaasAdminPermissions())
            {
                await userManager.AddClaimAsync(user, new System.Security.Claims.Claim("permission", permission));
            }
        }

        private async Task AssignGroupPermissionClaimsAsync(UserManager<User> userManager, User user, params string[] groups)
        {
            var existingClaims = await userManager.GetClaimsAsync(user);
            foreach (var claim in existingClaims.Where(c => c.Type == "permission").ToList())
            {
                await userManager.RemoveClaimAsync(user, claim);
            }

            var permissions = groups.Contains(PermissionGroupSeed.PatientRole)
                ? PermissionCatalog.PatientPermissions()
                : PermissionCatalog.PsychologistPermissions();

            foreach (var permission in permissions)
            {
                await userManager.AddClaimAsync(user, new System.Security.Claims.Claim("permission", permission));
            }
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole<UserId>> roleManager)
        {
            foreach (var role in new[] { PermissionGroupSeed.PsychologistRole, PermissionGroupSeed.PatientRole, PermissionGroupSeed.AdminRole })
            {
                if (await roleManager.RoleExistsAsync(role)) continue;

                var result = await roleManager.CreateAsync(new IdentityRole<UserId>(role));
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(error => error.Description));
                    throw new InvalidOperationException($"Nao foi possivel criar a role '{role}': {errors}");
                }
            }
        }

        private static async Task ApplySchemaPatchesAsync(ApplicationDbContext context, CancellationToken cancellationToken)
        {
            if (!await HasTableAsync(context, "auth", "users", cancellationToken)) return;

            await context.Database.ExecuteSqlRawAsync(
                "ALTER TABLE auth.users ADD COLUMN IF NOT EXISTS is_mfa_enabled boolean NOT NULL DEFAULT false;",
                cancellationToken);

            if (await HasTableAsync(context, "auth", "mfa_challenges", cancellationToken))
            {
                await context.Database.ExecuteSqlRawAsync(
                    "ALTER TABLE auth.mfa_challenges ADD COLUMN IF NOT EXISTS expires_at timestamp with time zone NOT NULL DEFAULT (NOW() + INTERVAL '10 minutes');",
                    cancellationToken);
            }
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
