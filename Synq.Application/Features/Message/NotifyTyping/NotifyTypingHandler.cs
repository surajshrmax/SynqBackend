using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;

namespace Synq.Application.Features.Message.NotifyTyping;

public class NotifyTypingHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    IConnectionStore connectionStore,
    IRealTimeMessageNotifier messageNotifier
) : IRequestHandler<NotifyTypingCommand>
{
  public async Task Handle(NotifyTypingCommand command, CancellationToken cancellationToken)
  {
    var chat = await dbContext.Chats
      .Where(c => c.Id == Guid.Parse(command.ChatId))
      .Include(c => c.ChatMembers)
      .FirstOrDefaultAsync(cancellationToken);

    if (chat == null)
    {
      return;
    }

    var recieverId = chat.ChatMembers.FirstOrDefault(cm => cm.UserId != currentUserService.UserId).UserId;

    if (recieverId != Guid.Empty &&
        connectionStore.TryGet(recieverId.ToString(), out string receiverConnectionId))
    {
      await messageNotifier.SendToUserAsync(receiverConnectionId, "StartTyping", currentUserService.UserId);
      Console.WriteLine("Sending typing indicator to " + receiverConnectionId);
    }
  }
}
