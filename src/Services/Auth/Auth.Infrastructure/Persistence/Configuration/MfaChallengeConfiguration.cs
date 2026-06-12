using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Infrastructure.Persistence.Configuration
{
    public class MfaChallengeConfiguration : IEntityTypeConfiguration<MfaChallenge>
    {
        public void Configure(EntityTypeBuilder<MfaChallenge> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(
                    id => id.Value,
                    value => new MfaChallengeId(value))
                .ValueGeneratedOnAdd();
        }
    }
}
