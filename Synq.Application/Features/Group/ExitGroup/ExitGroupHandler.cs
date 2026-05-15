using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Domain.Permissions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Synq.Application.Features.Group.ExitGroup;

public class ExitGroupHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService
) : IRequestHandler<ExitGroupCommnad>
{
    public async Task Handle(ExitGroupCommnad request, CancellationToken cancellationToken)
    {
        var isMember = await dbContext.ChatMembers
            .AsNoTracking()
            .Where(cm => cm.ChatId == Guid.Parse(request.GroupId) && 
            cm.UserId == currentUserService.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if(isMember != null && isMember.Role == Domain.Enums.GroupRole.Owner)
        {
            var firstAdmin = await dbContext.ChatMembers.Where(cm => cm.ChatId == Guid.Parse(request.GroupId) && cm.Role == Domain.Enums.GroupRole.Admin).FirstOrDefaultAsync(cancellationToken);
            if(firstAdmin == null)
            {
                throw new InvalidOperationException("Make someone admin before leaving this group.");
            }else
            {
                firstAdmin.Role = Domain.Enums.GroupRole.Owner;
                firstAdmin.Permissions = GroupRolePermissions.GetPermissions(Domain.Enums.GroupRole.Owner);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        if(isMember != null)
        {
            dbContext.ChatMembers.Remove(isMember);
            await dbContext.SaveChangesAsync(cancellationToken);
        }else
        {
            throw new UnauthorizedAccessException("You're not part of the group");
        }
    }
}
