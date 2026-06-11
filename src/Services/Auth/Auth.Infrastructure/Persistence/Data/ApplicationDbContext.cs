using Auth.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Persistence.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Consent> Consents => Set<Consent>();
        public DbSet<MfaChallenge> MfaChallenges => Set<MfaChallenge>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<PermissionGroup> PermissionGroups => Set<PermissionGroup>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("auth");

            builder.Entity<User>(b =>
            {
                b.ToTable("users");
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.TenantId).IsRequired();
                b.Property(x => x.Role).HasMaxLength(32).IsRequired();
                b.Property(x => x.Crp).HasMaxLength(32);
                b.Property(x => x.ConsentTermsVersion).HasMaxLength(32);
                b.Property(x => x.ConsentPrivacyVersion).HasMaxLength(32);
                b.Property(x => x.IsMfaEnabled).HasColumnName("is_mfa_enabled").IsRequired();
                b.Property(x => x.RefreshTokenHash).HasMaxLength(256);

                b.OwnsOne(x => x.Name, nb =>
                {
                    nb.Property(x => x.FirstName).HasColumnName("first_name").HasMaxLength(80).IsRequired();
                    nb.Property(x => x.LastName).HasColumnName("last_name").HasMaxLength(80).IsRequired();
                });

                b.OwnsOne(x => x.Contact, cb =>
                {
                    cb.Property(x => x.Email).HasColumnName("contact_email").HasMaxLength(254).IsRequired();
                    cb.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(32);
                });
            });

            builder.Entity<Consent>(b =>
            {
                b.ToTable("consents");
                b.HasKey(x => x.Id);
                b.Property(x => x.TermsVersion).HasMaxLength(32).IsRequired();
                b.Property(x => x.PrivacyVersion).HasMaxLength(32).IsRequired();
                b.Property(x => x.DocumentHash).HasMaxLength(128).IsRequired();
                b.HasIndex(x => new { x.UserId, x.TermsVersion, x.PrivacyVersion }).IsUnique();
            });

            builder.Entity<MfaChallenge>(b =>
            {
                b.ToTable("mfa_challenges");
                b.HasKey(x => x.Id);
                b.Property(x => x.SecretEncrypted).HasMaxLength(512);
            });

            builder.Entity<OutboxMessage>(b =>
            {
                b.ToTable("outbox_messages");
                b.HasKey(x => x.Id);
                b.Property(x => x.AggregateType).HasMaxLength(64).IsRequired();
                b.Property(x => x.EventType).HasMaxLength(128).IsRequired();
                b.Property(x => x.Payload).IsRequired();
            });

            builder.Entity<Permission>(b =>
            {
                b.ToTable("permissions");
                b.HasKey(x => x.Id);
                b.Property(x => x.GroupKey).HasMaxLength(64).IsRequired();
                b.Property(x => x.ClaimType).HasMaxLength(64).IsRequired();
                b.Property(x => x.ClaimValue).HasMaxLength(128).IsRequired();
                b.HasIndex(x => new { x.ClaimType, x.ClaimValue }).IsUnique();
            });

            builder.Entity<PermissionGroup>(b =>
            {
                b.ToTable("permission_groups");
                b.HasKey(x => x.Id);
                b.Property(x => x.GroupKey).HasMaxLength(64).IsRequired();
                b.Property(x => x.GroupName).HasMaxLength(160).IsRequired();
                b.HasIndex(x => x.GroupKey).IsUnique();
            });
        }
    }
}
