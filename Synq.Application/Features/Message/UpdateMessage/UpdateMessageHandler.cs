using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs;

namespace Synq.Application.Features.Message.UpdateMessage;

public class UpdateMessageHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService) : IRequestHandler<UpdateMessageCommand, MessageDto?>
{
    public async Task<MessageDto?> Handle(UpdateMessageCommand command, CancellationToken cancellationToken)
    {
        var messageId = Guid.Parse(command.MessageId);
        var message = await dbContext.Messages.Where(m => m.Id == messageId && m.SenderId == currentUserService.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (message == null)
        {
            return null;
        }

        message.Content = command.Content;
        await dbContext.SaveChangesAsync(cancellationToken);

        return new MessageDto(Id: message.Id, Content: message.Content, SenderId: message.SenderId,
            SentAt: message.SentAt);
    }
}