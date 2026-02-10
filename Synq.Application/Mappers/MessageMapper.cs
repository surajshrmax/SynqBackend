using Synq.Application.DTOs;
using Synq.Domain.Entities;

namespace Synq.Application.Mappers;

public static class MessageMapper
{
    public static MessageDto ToDto(this Message message)
    {
        return new MessageDto(Id: message.Id, Content: message.Content, ChatId: message.ChatId, Sender: message.Sender.ToDto(), SenderId: message.SenderId, SentAt: message.SentAt);
    }
}