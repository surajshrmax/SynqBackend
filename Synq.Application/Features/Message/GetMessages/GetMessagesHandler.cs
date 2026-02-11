using System.Runtime.InteropServices;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs;
using Synq.Application.DTOs.Message;

namespace Synq.Application.Features.Message.GetMessages;

public class GetMessagesHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService) : IRequestHandler<GetMessagesQuery, MessagePageResponse>
{
    const int pageSize = 15;
    public async Task<MessagePageResponse> Handle(GetMessagesQuery query, CancellationToken cancellationToken)
    {
        Guid chatId = Guid.Parse(query.ChatId);

        if (!query.IsChatId)
        {
            chatId = await dbContext.ChatMembers
                    .GroupBy(cm => cm.ChatId)
                    .Where(g =>
                            g.Any(cm => cm.UserId == currentUserService.UserId) &&
                            g.Any(cm => cm.UserId == chatId))
                    .Select(cm => cm.Key)
                    .FirstOrDefaultAsync(cancellationToken);
        }

        var messages = dbContext.Messages.AsQueryable();

        IQueryable<Domain.Entities.Message> messagePage;
        DateTime lastCursorTime;

        if (!string.IsNullOrWhiteSpace(query.LastCursorTime) && !query.LastCursorTime.Contains("null"))
        {
            var time = DateTimeOffset.Parse(query.LastCursorTime);
            messagePage = messages
              .Where(m => m.ChatId == chatId && m.SentAt > time)
              .OrderByDescending(m => m.SentAt)
              .Take(pageSize);
        }
        else
        {
            messagePage = messages
              .Where(m => m.ChatId == chatId)
              .OrderByDescending(m => m.SentAt)
              .Take(pageSize);
        }

        List<MessageDto> messageDto = await messagePage.Select(m => new MessageDto(Id: m.Id, Content: m.Content, IsEdited: m.IsEdited,ChatId: m.ChatId, Sender: new UserDto
        {
            Id = m.Sender.Id,
            Username = m.Sender.Username,
            Email = m.Sender.Email,
            UserProfile = new UserProfileDto
            {
                Name = m.Sender.UserProfile.Name,
                Bio = m.Sender.UserProfile.Bio,
                ImageUrl = m.Sender.UserProfile.ImageUrl,
                LastSeenAt = m.Sender.UserProfile.LastSeenAt
            }
        }, SenderId: m.SenderId, SentAt: m.SentAt)).ToListAsync(cancellationToken);

        lastCursorTime = messageDto[^1].SentAt;

        return new MessagePageResponse(messageDto, lastCursorTime);
    }
}
