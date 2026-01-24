using System.ComponentModel;
using MediatR;
using Synq.Application.Common.Interfaces;
using Synq.Domain.Enums;

namespace Synq.Application.Features.Message.SendMessage;

public class SendMessageHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService, IChatService chatService) : IRequestHandler<SendMessageCommand, Guid>
{
    public async Task<Guid> Handle(SendMessageCommand command, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId;

        if (command.Type == IdType.User)
        {
            var memeberId = Guid.Parse(command.Id);
            var chatId = await chatService.CreateOneToOneChatAsync(currentUserId, memeberId, cancellationToken);

            return await SendMessage(chatId, command.Content, currentUserId, cancellationToken);
        }
        
        if (command.Type == IdType.Chat)
        {
            return await SendMessage(Guid.Parse(command.Id), command.Content, currentUserId, cancellationToken);
        }

        throw new InvalidEnumArgumentException();
    }

    private async Task<Guid> SendMessage(Guid chatId, string content, Guid senderId, CancellationToken cancellationToken)
    {
        var message = new Domain.Entities.Message
        {
            ChatId = chatId,
            Content = content,
            MessageType = MessageType.Text,
            SenderId = senderId
        };

        await dbContext.Messages.AddAsync(message, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return message.Id;
    }
}