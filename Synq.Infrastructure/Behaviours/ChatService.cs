using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Domain.Entities;

namespace Synq.Infrastructure.Behaviours;

public class ChatService(IApplicationDbContext dbContext) : IChatService {
    public async Task<Guid> CreateOneToOneChatAsync(Guid currentUserId,Guid memberId, CancellationToken cancellationToken)
    {
        var chat = await dbContext.ChatMembers.AsNoTracking()
            .Where(cm => cm.UserId == currentUserId || cm.UserId == memberId)
            .GroupBy(cm => cm.ChatId)
            .Select(c => c.Key)
            .FirstOrDefaultAsync(cancellationToken);

        if (chat != Guid.Empty)
        {
            return chat;
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