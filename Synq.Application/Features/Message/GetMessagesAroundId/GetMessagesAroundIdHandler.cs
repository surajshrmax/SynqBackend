using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs;
using Synq.Application.DTOs.Message;
using Synq.Application.Mappers;

namespace Synq.Application.Features.Message.GetMessagesAroundId;

public class GetMessagesAroundIdHandler(IApplicationDbContext dbContext, IJsonHelper<MessageCursor> jsonHelper) : IRequestHandler<GetMessagesAroundIdQuery, MessagePageResponse>
{
  const int beforeMessagesSize = 5;
  const int afterMessagesSize = 5;
  public async Task<MessagePageResponse> Handle(GetMessagesAroundIdQuery query, CancellationToken cancellationToken)
  {
    var chatId = Guid.Parse(query.ChatId);
    var sentAt = DateTimeOffset.Parse(query.SentAt);
    var messageId = Guid.Parse(query.MessageId);

    bool hasMoreBefore = false;
    bool hasMoreAfter = false;
    string? beforeCursor = null;
    string? afterCursor = null;

    List<MessageDto> messagePage = [];

    var message = await dbContext.Messages
      .AsNoTracking()
      .Select(MessageMapper.toDtoExpr)
      .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);

    if (message == null)
    {
      throw new Exception("Message not found");
    }

    var before = await dbContext.Messages
          .AsNoTracking()
          .Where(m => m.ChatId == chatId && m.SentAt < sentAt)
          .OrderByDescending(m => m.SentAt)
          .Take(beforeMessagesSize + 1)
          .Select(MessageMapper.toDtoExpr)
          .ToListAsync(cancellationToken);

    var after = await dbContext.Messages
          .AsNoTracking()
          .Where(m => m.ChatId == chatId && m.SentAt > message.SentAt)
          .OrderBy(m => m.SentAt)
          .Take(afterMessagesSize + 1)
          .Select(MessageMapper.toDtoExpr)
          .ToListAsync(cancellationToken);


    if (before.Count > 0 && before.Count >= beforeMessagesSize)
    {
      before.RemoveAt(before.Count - 1);
      hasMoreBefore = true;
      beforeCursor = jsonHelper.Encode(new MessageCursor
      {
        MessageId = before[before.Count - 1].Id,
        SentAt = before[before.Count - 1].SentAt
      });

    }

    if (after.Count > 0 && after.Count >= afterMessagesSize)
    {
      after.RemoveAt(after.Count - 1);
      hasMoreAfter = true;
      afterCursor = jsonHelper.Encode(new MessageCursor
      {
        MessageId = after[after.Count - 1].Id,
        SentAt = after[after.Count - 1].SentAt
      });

    }

    messagePage.Add(message);
    messagePage.AddRange(after);
    messagePage.AddRange(before);

    return new MessagePageResponse
    {
      Messages = messagePage.OrderBy(m => m.SentAt),
      HasMoreBefore = hasMoreBefore,
      HasMoreAfter = hasMoreAfter,
      BeforeCursor = beforeCursor,
      AfterCursor = afterCursor
    };
  }
}
