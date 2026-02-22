using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs;
using Synq.Application.DTOs.Message;
using Synq.Application.Mappers;

namespace Synq.Application.Features.Message.GetOlderMessages;

public class GetOlderMessagesHandler(
    IApplicationDbContext dbContext,
    IJsonHelper<MessageCursor> jsonHelper
) : IRequestHandler<GetOlderMessagesQuery, MessagePageResponse>
{
  const int pageSize = 20;
  public async Task<MessagePageResponse> Handle(GetOlderMessagesQuery query, CancellationToken cancellationToken)
  {
    var chatId = Guid.Parse(query.ChatId);

    List<MessageDto> messageDtos;
    bool hasMore = false;
    MessageCursor? messageCursor = jsonHelper.Decode(query.Cursor);

    if (messageCursor == null)
    {
      return new MessagePageResponse { };
    }

    // DateTime messageSentTime = DateTimeOffset.Parse(messageCursor.SentAt);

    messageDtos = await dbContext.Messages.AsNoTracking()
                    .Where(m => m.ChatId == chatId && m.SentAt < messageCursor.SentAt)
                    .OrderByDescending(m => m.SentAt)
                    .Take(pageSize + 1)
                    .Select(m => new MessageDto
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
        HasMoreBefore = false,
      };
    }

    if (messageDtos.Count > pageSize)
    {
      messageDtos.RemoveAt(messageDtos.Count - 1);
      hasMore = true;
    }

    var cursor = jsonHelper.Encode(new MessageCursor
    {
      SentAt = messageDtos[messageDtos.Count - 1].SentAt,
      MessageId = messageDtos[messageDtos.Count - 1].Id
    });

    return new MessagePageResponse
    {
      BeforeCursor = cursor,
      HasMoreBefore = hasMore,
      Messages = messageDtos
    };
  }
}
