using Synq.Application.DTOs;
using Synq.Domain.Entities;

namespace Synq.Application.Mappers;

public static class MessageMapper
{
  public static MessageDto? ToDto(this Message message)
  {
    if (message == null) return null;
    return new MessageDto
    {
      Id = message.Id,
      Content = message.Content,
      IsEdited = message.IsEdited,
      ReplyMessageId = message.ReplyMessageId,
      ChatId = message.ChatId,
      Sender = message.Sender.ToDto(),
      SenderId = message.SenderId,
      SentAt = message.SentAt
    };
  }
}
