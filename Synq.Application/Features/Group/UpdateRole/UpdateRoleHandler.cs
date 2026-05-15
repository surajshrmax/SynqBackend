using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.Extensions;
using Synq.Domain.Enums;
using Synq.Domain.Permissions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Synq.Application.Features.Group.UpdateRole;

public class UpdateRoleHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService
) : IRequestHandler<UpdateRoleCommand>
{
    public async Task Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var isMember = await dbContext.ChatMembers
            .AsNoTracking()
            .Where(cm => cm.ChatId == Guid.Parse(request.GroupId) && cm.UserId == currentUserService.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        var memberToPromote = await dbContext.ChatMembers
            .Where(cm => cm.ChatId == Guid.Parse(request.GroupId) && cm.UserId == Guid.Parse(request.UserId))
            .FirstOrDefaultAsync(cancellationToken);

        if(memberToPromote == null)
        {
            throw new Exception("Member not found");
        }

        if(isMember != null && isMember.Permissions.HasPermission(Domain.Enums.GroupPermissions.ManageRoles))
        {
            GroupRole role = request.Role switch { 
                "admin" => GroupRole.Admin,
                "mod" => GroupRole.Mod,
                "owner" => GroupRole.Owner,
                "member" => GroupRole.Member,
                _ => memberToPromote.Role,
            };

            if(role == GroupRole.Owner)
            {
                isMember.Role = GroupRole.Member;
                isMember.Permissions = GroupRolePermissions.GetPermissions(GroupRole.Member);
            }

            memberToPromote.Role = role;
            memberToPromote.Permissions = GroupRolePermissions.GetPermissions(role);

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
