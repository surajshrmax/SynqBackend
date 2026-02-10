using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Domain.Entities;

namespace Synq.Infrastructure.Behaviours;

public class ChatService(IApplicationDbContext dbContext) : IChatService
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
}