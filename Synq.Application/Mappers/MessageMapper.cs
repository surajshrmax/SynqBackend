using System.Linq.Expressions;
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

  public static Expression<Func<Message, MessageDto>> toDtoExpr = m => new MessageDto
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
  };
}
