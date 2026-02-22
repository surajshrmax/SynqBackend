using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs;
using Synq.Application.DTOs.Message;
using Synq.Application.Mappers;

namespace Synq.Application.Features.Message.GetInitialMessages;

public class GetInitialMessagesHandler(
  IApplicationDbContext dbContext,
  ICurrentUserService currentUserService,
  IJsonHelper<MessageCursor> jsonHelper) : IRequestHandler<GetInitialMessagesQuery, MessagePageResponse>
{
  const int pageSize = 20;
  public async Task<MessagePageResponse> Handle(GetInitialMessagesQuery query, CancellationToken cancellationToken)
  {
    Guid chatId = Guid.Parse(query.ChatId);

    var messages = dbContext.Messages.AsQueryable();

    IQueryable<Domain.Entities.Message> messagePage;
    bool HasMore = false;

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

    messagePage = messages
      .Where(m => m.ChatId == chatId)
      .OrderByDescending(m => m.SentAt)
      .Take(pageSize + 1);

    List<MessageDto> messageDtos = await messagePage
      .Select(
          m => new MessageDto
          {
            Id = m.Id,
            Content = m.Content,
            IsEdited = m.IsEdited,
            ReplyMessageId = m.ReplyMessageId,
            Reply = m.ReplyMessage.ToDto(),
            ChatId = m.ChatId,
            Sender = new UserDto
            {
              Id = m.Sender.Id,
              Username = m.Sender.Username,
              UserProfile = new UserProfileDto
              {
                Name = m.Sender.UserProfile.Name,
                ImageUrl = m.Sender.UserProfile.ImageUrl,
              }
            },
            SenderId = m.SenderId,
            SentAt = m.SentAt
          })
      .ToListAsync(cancellationToken);

    if (messageDtos.Count <= 0)
    {
      return new MessagePageResponse
      {
        ChatId = chatId,
        HasMoreAfter = false,
        HasMoreBefore = false,
      };
    }

    if (messageDtos.Count > pageSize)
    {
      messageDtos.RemoveAt(messageDtos.Count - 1);
      HasMore = true;
    }

    var cursor = jsonHelper.Encode(new MessageCursor
    {
      SentAt = messageDtos[messageDtos.Count - 1].SentAt,
      MessageId = messageDtos[messageDtos.Count - 1].Id
    });

    return new MessagePageResponse
    {
      ChatId = chatId,
      HasMoreBefore = HasMore,
      BeforeCursor = cursor,
      Messages = messageDtos
    };
  }
}
