using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs;
using Synq.Application.Mappers;
using Synq.Domain.Entities;
using Synq.Domain.Enums;

namespace Synq.Application.Features.Message.SendMessage;

public class SendMessageHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    IChatService chatService,
    IRealTimeMessageNotifier messageNotifier,
    ICacheService cacheService,
    IJsonHelper<Chat> jsonHelper
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

      await SendMessage(chatId, command.Content, command.LocalId, currentUserId, replyMessageId, cancellationToken);
    }
    else
    {
      await SendMessage(Guid.Parse(command.Id), command.Content, command.LocalId, currentUserId, replyMessageId, cancellationToken);
    }
  }

  private async Task SendMessage(Guid chatId, string content, string localId, Guid senderId, Guid? replyMessageId, CancellationToken cancellationToken)
  {
    Chat? chat;

    string chatKey = $"chat {chatId}";

    var cachedChat = await cacheService.GetValueAsync(chatKey);


    if (cachedChat != null)
    {
      chat = jsonHelper.Decode(cachedChat);
    }
    else
    {
      chat = await dbContext.Chats
        .AsNoTracking()
        .Where(c => c.Id == chatId)
        .Select(c => new Chat
        {
          Id = c.Id,
          ChatMembers = c.ChatMembers.Select(cm => new ChatMember
          {
            UserId = cm.UserId
          }).ToList()
        })
        .FirstOrDefaultAsync(cancellationToken);

      await cacheService.SetValueAsync(chatKey, jsonHelper.Encode(chat), TimeSpan.FromMinutes(30));
    }

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

    await dbContext.MessageStatuses.AddAsync(new Domain.Entities.MessageStatus
    {
      MessageId = message.Id,
      UserId = recieverId
    }, cancellationToken);
    await dbContext.SaveChangesAsync(cancellationToken);

    var messageDto = new MessageDto
    {
      Id = message.Id,
      Content = message.Content,
      Reply = reply,
      SenderId = message.SenderId,
      SentAt = message.SentAt,
    };

    await messageNotifier.SendToUserAsync(
        recieverId.ToString(),
        "RecieveMessage",
        messageDto
    );

    await messageNotifier.SendToUserAsync(
        currentUserService.UserId.ToString(),
        "MessageSent",
        new MessageDto
        {
          Id = message.Id,
          LocalId = localId,
          Status = message.Status.Status,
          SentAt = message.SentAt
        });
  }
}
