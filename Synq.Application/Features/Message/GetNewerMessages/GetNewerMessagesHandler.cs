using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs.Message;
using Synq.Application.Mappers;

namespace Synq.Application.Features.Message.GetNewerMessages;

public class GetNewerMessagesHandler(
    IApplicationDbContext dbContext,
    IJsonHelper<MessageCursor> jsonHelper
) : IRequestHandler<GetNewerMessagesQuery, MessagePageResponse>
{
  const int pageSize = 20;
  public async Task<MessagePageResponse> Handle(GetNewerMessagesQuery query, CancellationToken cancellationToken)
  {
    bool hasMoreAfter = false;
    string? afterCursor = null;

    Guid chatId = Guid.Parse(query.ChatId);

    if (query.Cursor == null)
    {
      return new MessagePageResponse { };
    }

    MessageCursor? cursor = jsonHelper.Decode(query.Cursor!);

    if (cursor == null)
    {
      return new MessagePageResponse { };
    }

    var messages = await dbContext.Messages
      .AsNoTracking()
      .Where(m => m.ChatId == chatId && m.SentAt > cursor.SentAt)
      .OrderBy(m => m.SentAt)
      .Take(pageSize + 1)
      .Select(MessageMapper.toDtoExpr)
      .ToListAsync(cancellationToken);

    if (messages.Count > 0 && messages.Count >= pageSize)
    {
      messages.RemoveAt(messages.Count - 1);
      hasMoreAfter = true;

      afterCursor = jsonHelper.Encode(new MessageCursor
      {
        MessageId = messages[messages.Count - 1].Id,
        SentAt = messages[messages.Count - 1].SentAt
      });
    }

    return new MessagePageResponse
    {
      AfterCursor = afterCursor,
      HasMoreAfter = hasMoreAfter,
      Messages = messages
    };
  }
}
