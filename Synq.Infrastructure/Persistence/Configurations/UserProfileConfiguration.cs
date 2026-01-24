using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Synq.Domain.Entities;

namespace Synq.Infrastructure.Persistence.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.HasKey(p => p.UserId);

        builder.HasOne(p => p.User)
            .WithOne(p => p.UserProfile)
            .HasForeignKey<UserProfile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(60);
        builder.Property(p => p.Bio).HasDefaultValue("").HasMaxLength(200);
        builder.Property(p => p.ImageUrl).HasDefaultValue("");

        builder.Property(p => p.LastSeenAt).IsRequired().HasDefaultValueSql("now()");
    }
}