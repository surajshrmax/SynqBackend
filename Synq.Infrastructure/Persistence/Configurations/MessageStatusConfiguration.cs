using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Synq.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Synq.Infrastructure.Persistence.Configurations;

public class MessageStatusConfiguration : IEntityTypeConfiguration<MessageStatus>
{
    public void Configure(EntityTypeBuilder<MessageStatus> builder)
    {
        builder.HasKey(c => new { c.MessageId, c.UserId});
        builder.Property(c => c.Status).HasDefaultValue("sent");

        builder.HasOne(c => c.User).WithMany(c => c.MessageStatuses).HasForeignKey(c => c.UserId);
        builder.HasOne(c => c.Message).WithOne(c => c.Status).HasForeignKey<MessageStatus>(c => c.MessageId).OnDelete(DeleteBehavior.Cascade);
    }
}
