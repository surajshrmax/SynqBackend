using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Domain.Entities;

namespace Synq.Infrastructure.Behaviours;

public class ChatService(
    IApplicationDbContext dbContext,
    ICacheService cacheService,
    IJsonHelper<Chat> jsonHelper) : IChatService
{
  public async Task<Guid> CreateOneToOneChatAsync(Guid currentUserId, Guid memberId, CancellationToken cancellationToken)
  {
    var chatId = await dbContext.ChatMembers
        .GroupBy(cm => cm.ChatId)
        .Where(g =>
            g.Any(cm => cm.UserId == currentUserId) &&
            g.Any(cm => cm.UserId == memberId))
        .Select(g => g.Key)
        .FirstOrDefaultAsync(cancellationToken);

    if (chatId != Guid.Empty)
    {
      return chatId;
    }

    var newChat = new Chat
    {
      IsGroup = false,
      ChatMembers = new List<ChatMember>
            {
                new ChatMember{UserId = currentUserId},
                new ChatMember{UserId = memberId}
            }
    };

    await dbContext.Chats.AddAsync(newChat, cancellationToken);
    await dbContext.SaveChangesAsync(cancellationToken);

    return newChat.Id;
  }

  public async Task<bool> Exists(Guid chatId)
  {
    return GetChatAsync(chatId) != null;
  }

  public async Task<Chat?> GetChatAsync(Guid chatId)
  {
    string key = $"chat {chatId}";
    var cachedChat = await cacheService.GetValueAsync(key);

    if (cachedChat != null)
    {
      return jsonHelper.Decode(cachedChat);
    }
    else
    {
      var chat = await dbContext.Chats
        .AsNoTracking()
        .Where(c => c.Id == chatId)
        .Select(c => new Chat
        {
          Id = c.Id,
          ChatMembers = c.ChatMembers
        })
        .FirstOrDefaultAsync();

      if (chat != null)
      {
        await cacheService.SetValueAsync(key, jsonHelper.Encode(chat), TimeSpan.FromMinutes(30));
        return chat;
      }

      return null;
    }
  }

  public async Task<bool> IsAMember(Guid chatId, Guid userId)
  {
    var chat = await GetChatAsync(chatId);

    return chat?.ChatMembers.Any(cm => cm.UserId == userId) == true;
  }
}
