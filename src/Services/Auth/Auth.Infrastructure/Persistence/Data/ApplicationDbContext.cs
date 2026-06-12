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

        public DbSet<User> Users { get; set; }
        public DbSet<Consent> Consents { get; set; }
        public DbSet<MfaChallenge> MfaChallenges { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionGroup> PermissionGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new ConsentConfiguration());
            builder.ApplyConfiguration(new MfaChallengeConfiguration());
            builder.ApplyConfiguration(new OutboxMessageConfiguration());
            builder.ApplyConfiguration(new PermissionConfiguration());
            builder.ApplyConfiguration(new PermissionGroupConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
