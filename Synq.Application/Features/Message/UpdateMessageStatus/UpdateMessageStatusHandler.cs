using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs;
using Synq.Domain.Entities;

namespace Synq.Application.Features.Message.UpdateMessageStatus;

public class UpdateMessageStatusHandler(
    IApplicationDbContext dbContext, 
    ICurrentUserService currentUserService,
    IConnectionStore connectionStore,
    IRealTimeMessageNotifier messageNotifier
    ) : IRequestHandler<UpdateMessageStatusCommand>
{
    public async Task Handle(UpdateMessageStatusCommand command, CancellationToken cancellationToken)
    {
        var message = await dbContext.Messages.Where(m => m.Id == Guid.Parse(command.MessageId) && m.Status.UserId == currentUserService.UserId).Include(m => m.Status).FirstOrDefaultAsync();
        if (message == null) return;
        message.Status.Status = command.Status;
        await dbContext.SaveChangesAsync(cancellationToken);

        if(connectionStore.TryGet(message.SenderId.ToString(), out var senderConnectionId))
        {
            await messageNotifier.SendToUserAsync(senderConnectionId, "MessageUpdate", 
                new MessageDto {
                    Id = message.Id,
                    Status = message.Status.Status,
            });
        }
    }
}