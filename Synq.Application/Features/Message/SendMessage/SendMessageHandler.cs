using System.ComponentModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs;
using Synq.Application.DTOs.Message;
using Synq.Application.Mappers;
using Synq.Domain.Enums;

namespace Synq.Application.Features.Message.SendMessage;

public class SendMessageHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService, IChatService chatService) : IRequestHandler<SendMessageCommand, MessageResponse>
{
  public async Task<MessageResponse> Handle(SendMessageCommand command, CancellationToken cancellationToken)
  {
    var currentUserId = currentUserService.UserId;
    Guid? replyMessageId = null;

    if (command.ReplyMessageId != null)
    {
      replyMessageId = Guid.Parse(command.ReplyMessageId);
    }

    if (command.Type == IdType.User)
    {
      var memeberId = Guid.Parse(command.Id);
      var chatId = await chatService.CreateOneToOneChatAsync(currentUserId, memeberId, cancellationToken);

      return await SendMessage(chatId, command.Content, currentUserId, replyMessageId, cancellationToken);
    }

    if (command.Type == IdType.Chat)
    {
      return await SendMessage(Guid.Parse(command.Id), command.Content, currentUserId, replyMessageId, cancellationToken);
    }

    throw new InvalidEnumArgumentException();
  }

  private async Task<MessageResponse> SendMessage(Guid chatId, string content, Guid senderId, Guid? replyMessageId, CancellationToken cancellationToken)
  {
    var chat = await dbContext.Chats.AsNoTracking().Where(c => c.Id == chatId).Include(c => c.ChatMembers).FirstOrDefaultAsync(cancellationToken);
    Domain.Entities.Message message;
    MessageDto? reply = null;

    if (chat == null || chat.ChatMembers.FirstOrDefault(cm => cm.UserId == senderId) == null)
    {
      throw new Exception("Chat does not exists");
    }

    if (replyMessageId == null)
    {
      message = new Domain.Entities.Message
      {
        ChatId = chatId,
        Content = content,
        MessageType = MessageType.Text,
        SenderId = senderId,
      };
    }
    else
    {
      reply = await dbContext.Messages.AsNoTracking().Where(m => m.Id == replyMessageId!).Select(m => new MessageDto
      {
        Id = m.Id,
        Content = m.Content
      }).FirstOrDefaultAsync(cancellationToken);

      message = new Domain.Entities.Message
      {
        ChatId = chatId,
        Content = content,
        MessageType = MessageType.Text,
        SenderId = senderId,
        ReplyMessageId = replyMessageId
      };
    }

    await dbContext.Messages.AddAsync(message, cancellationToken);
    await dbContext.SaveChangesAsync(cancellationToken);

    var recieverId = chat.ChatMembers.First(cm => cm.UserId != senderId).UserId;
    var sender = await dbContext.Users.AsNoTracking().Where(u => u.Id == senderId).Include(u => u.UserProfile).Select(UserMapper.ToDtoExpr).FirstAsync(cancellationToken);

    return new MessageResponse(
        RecieverId: recieverId,
        Message: new MessageDto
        {
          Id = message.Id,
          Content = message.Content,
          IsEdited = message.IsEdited,
          ReplyMessageId = message.ReplyMessageId,
          Reply = reply,
          ChatId = message.ChatId,
          Sender = sender,
          SenderId = message.SenderId,
          SentAt = message.SentAt
        }
    );
  }
}
