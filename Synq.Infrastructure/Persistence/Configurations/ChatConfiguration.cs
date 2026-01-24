using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Synq.Domain.Entities;

namespace Synq.Infrastructure.Persistence.Configurations;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(c => c.IsGroup).IsRequired();
        builder.Property(c => c.Title).HasMaxLength(50).HasDefaultValue("");

        builder.Property(c => c.CreatedAt).HasDefaultValueSql("now()");
    }
}