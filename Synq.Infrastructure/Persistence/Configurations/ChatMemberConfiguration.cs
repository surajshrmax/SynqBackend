    using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Synq.Domain.Entities;

namespace Synq.Infrastructure.Persistence.Configurations;

public class ChatMemberConfiguration : IEntityTypeConfiguration<ChatMember>
{
    public void Configure(EntityTypeBuilder<ChatMember> builder)
    {
        builder.HasKey(cm => new { cm.UserId, cm.ChatId });

        builder.Property(cm => cm.IsAdmin).HasDefaultValue(false);

        builder.HasOne(cm => cm.User).WithMany(u => u.ChatMembers).HasForeignKey(cm => cm.UserId);
        builder.HasOne(cm => cm.Chat).WithMany(c => c.ChatMembers).HasForeignKey(cm => cm.ChatId);

        builder.Property(cm => cm.CreatedAt).HasDefaultValueSql("now()");
    }
}