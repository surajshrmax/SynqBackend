using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;

namespace Synq.Application.Features.Group.AddMember;

public class AddMemberHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService) : IRequestHandler<AddMemberCommand>
{
    public async Task Handle(AddMemberCommand request, CancellationToken cancellationToken)
    {
        var isValidRequest = await dbContext.ChatMembers.AsNoTracking().Where(cm => cm.ChatId == Guid.Parse(request.GroupId) && cm.UserId == currentUserService.UserId).FirstOrDefaultAsync(cancellationToken);
        if(isValidRequest == null)
        {
            throw new UnauthorizedAccessException("You're not part of the group");
        }


        await dbContext.ChatMembers.AddAsync(new Domain.Entities.ChatMember {
            ChatId = Guid.Parse(request.GroupId),
            UserId = Guid.Parse(request.UserId),
            IsAdmin = false
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
