using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs;

namespace Synq.Application.Features.Message.GetMessages;

public class GetMessagesHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService) : IRequestHandler<GetMessagesQuery, IEnumerable<MessageDto>>
{
    public async Task<IEnumerable<MessageDto>> Handle(GetMessagesQuery query, CancellationToken cancellationToken)
    {
        var messages = await dbContext.Messages.AsNoTracking().Where(m => m.ChatId == Guid.Parse(query.ChatId))
            .Select(m => new MessageDto(Id: m.Id, Content: m.Content, SenderId: m.SenderId, SentAt: m.SentAt))
            .ToListAsync(cancellationToken);

        return messages;
    }
}
