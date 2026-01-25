using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;

namespace Synq.Application.Features.Message.DeleteMessage;

public class DeleteMessageHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService) : IRequestHandler<DeleteMessageCommand, bool>
{
    public async Task<bool> Handle(DeleteMessageCommand command, CancellationToken cancellationToken)
    {
        var messageId = Guid.Parse(command.MessageId);
        var message = await dbContext.Messages.Where(m => m.Id == messageId && m.SenderId == currentUserService.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (message == null)
        {
            return false;
        }

        dbContext.Messages.Remove(message);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}