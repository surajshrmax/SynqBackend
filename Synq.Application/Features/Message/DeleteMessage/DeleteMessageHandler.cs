using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Domain.Entities;

namespace Synq.Application.Features.Message.DeleteMessage;

public class DeleteMessageHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    IRealTimeMessageNotifier messageNotifier,
    ICacheService cacheService,
    IJsonHelper<Chat> jsonHelper
    ) : IRequestHandler<DeleteMessageCommand>
{
  public async Task Handle(DeleteMessageCommand command, CancellationToken cancellationToken)
  {
    var messageId = Guid.Parse(command.MessageId);
    var message = await dbContext.Messages.Where(m => m.Id == messageId && m.SenderId == currentUserService.UserId)
        .FirstOrDefaultAsync(cancellationToken);

    if (message == null)
    {
      throw new Exception("Message doest not exits");
    }

    dbContext.Messages.Remove(message);
    await dbContext.SaveChangesAsync(cancellationToken);

    Chat? chat;
    string chatKey = $"chat {message.ChatId}";

    var cachedChat = await cacheService.GetValueAsync(chatKey);

    if (cachedChat != null)
    {
      chat = jsonHelper.Decode(cachedChat);
    }
    else
    {
      chat = await dbContext.Chats
        .AsNoTracking()
        .Where(c => c.Id == message.ChatId)
        .Select(c => new Chat
        {
          Id = c.Id,
          ChatMembers = c.ChatMembers.Select(cm => new ChatMember
          {
            UserId = cm.UserId
          }).ToList()
        })
        .FirstOrDefaultAsync(cancellationToken);

      await cacheService.SetValueAsync(chatKey, jsonHelper.Encode(chat), TimeSpan.FromMinutes(30));
    }

    var recieverId = chat.ChatMembers.FirstOrDefault(cm => cm.UserId != currentUserService.UserId);

    await messageNotifier.SendToUserAsync(recieverId.ToString(), "MessageDeleted", messageId);
    await messageNotifier.SendToUserAsync(currentUserService.UserId.ToString(), "MessageDeleted", messageId);
  }
}
