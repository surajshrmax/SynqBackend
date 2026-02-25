using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs;
using Synq.Application.Mappers;

namespace Synq.Application.Features.Message.UpdateMessage;

public class UpdateMessageHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    IConnectionStore connectionStore,
    IRealTimeMessageNotifier messageNotifier
) : IRequestHandler<UpdateMessageCommand>
{
  public async Task Handle(UpdateMessageCommand command, CancellationToken cancellationToken)
  {
    var messageId = Guid.Parse(command.MessageId);
    var message = await dbContext.Messages
        .Where(m => m.Id == messageId && m.SenderId == currentUserService.UserId)
        .Include(c => c.Sender)
        .ThenInclude(c => c.UserProfile)
        .FirstOrDefaultAsync(cancellationToken);

    if (message == null)
    {
      return;
    }

    message.Content = command.Content;
    message.IsEdited = true;
    await dbContext.SaveChangesAsync(cancellationToken);

    var chat = await dbContext.Chats
                .Where(c => c.Id == message.ChatId)
                .Include(c => c.ChatMembers)
                .FirstOrDefaultAsync(cancellationToken);

    var recieverId = chat?.ChatMembers?.FirstOrDefault(m => m.UserId != currentUserService.UserId)?.UserId;

    if (recieverId != null &&
        recieverId != Guid.Empty &&
        connectionStore.TryGet(recieverId.ToString(), out string recieverConnectionId))
    {
      await messageNotifier.SendToUserAsync(recieverConnectionId, "MessageUpdate", message.ToDto());
    }

    if (connectionStore.TryGet(currentUserService.UserId.ToString(), out string senderConnectionId))
    {
      await messageNotifier.SendToUserAsync(senderConnectionId, "UpdateDone", message.ToDto());
    }
  }
}
