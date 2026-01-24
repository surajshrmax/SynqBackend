using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Synq.Domain.Entities;

namespace Synq.Infrastructure.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(m => m.Content).IsRequired();
        builder.Property(m => m.MessageType).HasConversion<string>();

        builder.HasOne(m => m.Sender).WithMany().HasForeignKey(m => m.SenderId);
        builder.HasOne(m => m.Chat).WithMany(c => c.Messages).HasForeignKey(m => m.ChatId);

        builder.Property(m => m.SentAt).HasDefaultValueSql("now()");
    }
}