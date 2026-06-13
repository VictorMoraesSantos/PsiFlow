using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Auth.Infrastructure.Persistence.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Persistence.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<UserId>, UserId>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Consent> Consents { get; set; }
        public DbSet<MfaChallenge> MfaChallenges { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionGroup> PermissionGroups { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Ignore<UserId>();
            builder.Ignore<TenantId>();
            builder.Ignore<PermissionId>();
            builder.Ignore<PermissionGroupId>();
            builder.Ignore<RefreshTokenId>();
            builder.ApplyConfiguration(new ConsentConfiguration());
            builder.ApplyConfiguration(new MfaChallengeConfiguration());
            builder.ApplyConfiguration(new OutboxMessageConfiguration());
            builder.ApplyConfiguration(new PermissionConfiguration());
            builder.ApplyConfiguration(new PermissionGroupConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new RefreshTokenConfiguration());

            builder.Entity<IdentityRole<UserId>>(entity =>
            {
                entity.Property(x => x.Id)
                    .HasConversion(id => id.Value, value => new UserId(value))
                    .ValueGeneratedOnAdd();
            });

            builder.Entity<IdentityUserClaim<UserId>>(entity =>
            {
                entity.Property(x => x.UserId)
                    .HasConversion(id => id.Value, value => new UserId(value));
            });

            builder.Entity<IdentityUserLogin<UserId>>(entity =>
            {
                entity.Property(x => x.UserId)
                    .HasConversion(id => id.Value, value => new UserId(value));
            });

            builder.Entity<IdentityUserRole<UserId>>(entity =>
            {
                entity.Property(x => x.UserId)
                    .HasConversion(id => id.Value, value => new UserId(value));
                entity.Property(x => x.RoleId)
                    .HasConversion(id => id.Value, value => new UserId(value));
            });

            builder.Entity<IdentityUserToken<UserId>>(entity =>
            {
                entity.Property(x => x.UserId)
                    .HasConversion(id => id.Value, value => new UserId(value));
            });

            builder.Entity<IdentityRoleClaim<UserId>>(entity =>
            {
                entity.Property(x => x.RoleId)
                    .HasConversion(id => id.Value, value => new UserId(value));
            });
        }
    }
}
