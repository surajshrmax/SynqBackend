using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs;
using Synq.Domain.Entities;
using Synq.Domain.Enums;

namespace Synq.Application.Features.Message.SendMessage;

public class SendMessageHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    IChatService chatService
    ) : IRequestHandler<SendMessageCommand, MessageDto>
{
  public async Task<MessageDto> Handle(SendMessageCommand command, CancellationToken cancellationToken)
  {
    var currentUserId = currentUserService.UserId;
    var chatId = Guid.Parse(command.Id);

    Guid? replyMessageId = null;

    if (command.ReplyToMessageId != null)
    {
      replyMessageId = Guid.Parse(command.ReplyToMessageId);
    }

    var chat = await chatService.GetChatAsync(chatId);

    if (chat?.ChatMembers.Any(cm => cm.UserId == currentUserId) == false)
    {
      throw new Exception("Chat does not exists");
    }

    MessageDto? reply = await dbContext.Messages.AsNoTracking().Where(m => m.Id == replyMessageId!).Select(m => new MessageDto
    {
      Id = m.Id,
      Content = m.Content
    }).FirstOrDefaultAsync(cancellationToken);

    var message = new Domain.Entities.Message
    {
      ChatId = chatId,
      Content = command.Content,
      MessageType = MessageType.Text,
      SenderId = currentUserId,
      ReplyMessageId = reply?.Id
    };

    await dbContext.Messages.AddAsync(message, cancellationToken);
    await dbContext.SaveChangesAsync(cancellationToken);

    var recieverId = chat.ChatMembers.First(cm => cm.UserId != currentUserId).UserId;

    await dbContext.MessageStatuses.AddAsync(new MessageStatus
    {
      MessageId = message.Id,
      UserId = recieverId
    }, cancellationToken);

    await dbContext.SaveChangesAsync(cancellationToken);

    return new MessageDto
    {
      LocalId = command.LocalId,
      Id = message.Id,
      Content = message.Content,
      Reply = reply,
      SenderId = message.SenderId,
      SentAt = message.SentAt,
    };
  }
}
