using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.Mappers;
using Synq.Domain.Entities;

namespace Synq.Application.Features.Message.UpdateMessage;

public class UpdateMessageHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    IRealTimeMessageNotifier messageNotifier,
    ICacheService cacheService,
    IJsonHelper<Chat> jsonHelper
) : IRequestHandler<UpdateMessageCommand>
{
  public async Task Handle(UpdateMessageCommand command, CancellationToken cancellationToken)
  {
    var messageId = Guid.Parse(command.MessageId);

    var message = await dbContext.Messages
        .Where(m => m.Id == messageId && m.SenderId == currentUserService.UserId)
        .Include(c => c.Sender)
        .ThenInclude(c => c.UserProfile)
        .FirstOrDefaultAsync(cancellationToken);

    if (message == null)
    {
      return;
    }

    message.Content = command.Content;
    message.IsEdited = true;
    await dbContext.SaveChangesAsync(cancellationToken);

    Chat? chat;

    string key = $"chat {message.ChatId}";

    var cachedChat = await cacheService.GetValueAsync(key);

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

      await cacheService.SetValueAsync(key, jsonHelper.Encode(chat), TimeSpan.FromMinutes(30));
    }


    var recieverId = chat?.ChatMembers?.FirstOrDefault(m => m.UserId != currentUserService.UserId)?.UserId;

    await messageNotifier.SendToUserAsync(recieverId.ToString(), "MessageUpdate", message.ToDto());
    await messageNotifier.SendToUserAsync(currentUserService.UserId.ToString(), "UpdateDone", message.ToDto());
  }
}
