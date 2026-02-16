using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs;
using Synq.Application.Mappers;

namespace Synq.Application.Features.Message.UpdateMessage;

public class UpdateMessageHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService) : IRequestHandler<UpdateMessageCommand, (Guid?, MessageDto?)>
{
  public async Task<(Guid?, MessageDto?)> Handle(UpdateMessageCommand command, CancellationToken cancellationToken)
  {
    var messageId = Guid.Parse(command.MessageId);
    var message = await dbContext.Messages
        .Where(m => m.Id == messageId && m.SenderId == currentUserService.UserId)
        .Include(c => c.Sender)
        .ThenInclude(c => c.UserProfile)
        .FirstOrDefaultAsync(cancellationToken);

    if (message == null)
    {
      return (Guid.Empty, null);
    }

    message.Content = command.Content;
    message.IsEdited = true;
    await dbContext.SaveChangesAsync(cancellationToken);

    var chat = await dbContext.Chats
                .Where(c => c.Id == message.ChatId)
                .Include(c => c.ChatMembers)
                .FirstOrDefaultAsync(cancellationToken);

    var recieverId = chat?.ChatMembers?.FirstOrDefault(m => m.UserId != currentUserService.UserId)?.UserId;

    return (recieverId, message.ToDto());
  }
}
