using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Domain.Entities;

namespace Synq.Application.Features.Message.DeleteMessage;

public class DeleteMessageHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService
    ) : IRequestHandler<DeleteMessageCommand, Guid?>
{
  public async Task<Guid?> Handle(DeleteMessageCommand command, CancellationToken cancellationToken)
  {
    var messageId = Guid.Parse(command.MessageId);
    var message = await dbContext.Messages
      .AsNoTracking()
      .Where(m => m.Id == messageId && m.SenderId == currentUserService.UserId)
      .FirstOrDefaultAsync(cancellationToken);

    if (message == null)
    {
      throw new Exception("Message doest not exits");
    }

    dbContext.Messages.Remove(message);
    await dbContext.SaveChangesAsync(cancellationToken);

    return message.ChatId;
  }
}
