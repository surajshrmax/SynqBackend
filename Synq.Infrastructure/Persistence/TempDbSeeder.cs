using Synq.Application.Common.Interfaces;
using Synq.Domain.Entities;
using Synq.Domain.Enums;

namespace Synq.Infrastructure.Persistence;

public static class TempDbSeeder
{
  private static readonly Guid chatId = Guid.Parse("5c5a353c-bff1-49ab-9302-e786bcc47518");
  private static readonly Guid senderId = Guid.Parse("7fcbc99b-ad94-4075-b581-50be9e67b226");

  public static void Seed(AppDbContext dbContext)
  {
    if (!dbContext.Messages.Any())
    {
      var messages = new List<Message>();

      for (int i = 0; i < 100; i++)
      {
        messages.Add(
            new Message
            {
              Content = $"{i}",
              MessageType = MessageType.Text,
              ChatId = chatId,
              SenderId = senderId,
              SentAt = DateTime.UtcNow.AddMinutes(-i)
            }
        );
      }

      dbContext.Messages.AddRange(messages);
      dbContext.SaveChanges();
    }
  }
}
