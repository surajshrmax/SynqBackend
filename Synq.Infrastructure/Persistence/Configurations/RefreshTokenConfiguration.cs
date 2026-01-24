using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Synq.Domain.Entities;

namespace Synq.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(t => t.Token).IsRequired().HasMaxLength(255);
        builder.Property(t => t.IsRevoked).HasDefaultValue(false);
        builder.HasIndex(t => t.Token).IsUnique();

        builder.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId);
    }
}