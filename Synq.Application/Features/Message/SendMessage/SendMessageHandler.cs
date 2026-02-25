using System.ComponentModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs;
using Synq.Application.DTOs.Message;
using Synq.Application.Mappers;
using Synq.Domain.Enums;

namespace Synq.Application.Features.Message.SendMessage;

public class SendMessageHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    IChatService chatService,
    IConnectionStore connectionStore,
    IRealTimeMessageNotifier messageNotifier
    ) : IRequestHandler<SendMessageCommand>
{
  public async Task Handle(SendMessageCommand command, CancellationToken cancellationToken)
  {
    var currentUserId = currentUserService.UserId;
    Guid? replyMessageId = null;

    if (command.ReplyToMessageId != null)
    {
      replyMessageId = Guid.Parse(command.ReplyToMessageId);
    }

    if (!command.IsChat)
    {
      var memeberId = Guid.Parse(command.Id);
      var chatId = await chatService.CreateOneToOneChatAsync(currentUserId, memeberId, cancellationToken);

      await SendMessage(chatId, command.Content, currentUserId, replyMessageId, cancellationToken);
    }
    else
    {
      await SendMessage(Guid.Parse(command.Id), command.Content, currentUserId, replyMessageId, cancellationToken);
    }
  }

  private async Task SendMessage(Guid chatId, string content, Guid senderId, Guid? replyMessageId, CancellationToken cancellationToken)
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
    var messageDto = new MessageDto
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
    };

    if (connectionStore.TryGet(recieverId.ToString(), out string recieverConnectionId))
    {
      await messageNotifier.SendToUserAsync(
          recieverConnectionId,
          "RecieveMessage",
          messageDto
      );
    }

    if (connectionStore.TryGet(message.SenderId.ToString(), out string senderConnectionId))
    {
      await messageNotifier.SendToUserAsync(
          senderConnectionId,
          "RecieveMessage",
          messageDto
      );
    }
  }
}
