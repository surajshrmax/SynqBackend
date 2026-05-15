using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.Extensions;

namespace Synq.Application.Features.Group.RemoveMember;

public class RemoveMemberHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService
) : IRequestHandler<RemoveMemberCommand>
{
    public async Task Handle(RemoveMemberCommand request, CancellationToken cancellationToken)
    {
        var isValidRequest = await dbContext.ChatMembers.AsNoTracking().Where(cm => cm.ChatId == Guid.Parse(request.GroupId) && cm.UserId == currentUserService.UserId).FirstOrDefaultAsync(cancellationToken);
        if (isValidRequest != null && isValidRequest.Permissions.HasPermission(Domain.Enums.GroupPermissions.KickMembers))
        {
            var member = await dbContext.ChatMembers.AsNoTracking().Where(cm => cm.ChatId == Guid.Parse(request.GroupId) && cm.UserId == Guid.Parse(request.UserId)).FirstOrDefaultAsync(cancellationToken);
            if (member == null)
            {
                return;
            }

            dbContext.ChatMembers.Remove(member);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            throw new UnauthorizedAccessException("You're not allowed to perform this operation.");
        }
    }
}