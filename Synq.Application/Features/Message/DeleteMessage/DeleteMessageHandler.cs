using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;

namespace Synq.Application.Features.Message.DeleteMessage;

public class DeleteMessageHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    IConnectionStore connectionStore,
    IRealTimeMessageNotifier messageNotifier
    ) : IRequestHandler<DeleteMessageCommand>
{
  public async Task Handle(DeleteMessageCommand command, CancellationToken cancellationToken)
  {
    var messageId = Guid.Parse(command.MessageId);
    var message = await dbContext.Messages.Where(m => m.Id == messageId && m.SenderId == currentUserService.UserId)
        .FirstOrDefaultAsync(cancellationToken);

    if (message == null)
    {
      throw new Exception("Message doest not exits");
    }

    dbContext.Messages.Remove(message);
    await dbContext.SaveChangesAsync(cancellationToken);

    var chat = await dbContext.Chats.Where(c => c.Id == message.ChatId).Include(c => c.ChatMembers).FirstOrDefaultAsync(cancellationToken);
    var recieverId = chat.ChatMembers.FirstOrDefault(cm => cm.UserId != currentUserService.UserId);

    if (recieverId != null && connectionStore.TryGet(recieverId.ToString()!, out string recieverConnectionId))
    {
      await messageNotifier.SendToUserAsync(recieverConnectionId, "MessageDeleted", messageId);
    }

    if (connectionStore.TryGet(currentUserService.UserId.ToString(), out string senderConnectionId))
    {
      await messageNotifier.SendToUserAsync(senderConnectionId, "MessageDeleted", messageId);
    }
  }
}
