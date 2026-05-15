using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.Extensions;
using Synq.Domain.Permissions;

namespace Synq.Application.Features.Group.AddMember;

public class AddMemberHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService) : IRequestHandler<AddMemberCommand>
{
    public async Task Handle(AddMemberCommand request, CancellationToken cancellationToken)
    {
        var isValidRequest = await dbContext.ChatMembers.AsNoTracking().Where(cm => cm.ChatId == Guid.Parse(request.GroupId) && cm.UserId == currentUserService.UserId).FirstOrDefaultAsync(cancellationToken);
        if (isValidRequest != null && isValidRequest.Permissions.HasPermission(Domain.Enums.GroupPermissions.AddMembers))
        {
            await dbContext.ChatMembers.AddAsync(new Domain.Entities.ChatMember
            {
                ChatId = Guid.Parse(request.GroupId),
                UserId = Guid.Parse(request.UserId),
                Role = Domain.Enums.GroupRole.Member,
                Permissions = GroupRolePermissions.GetPermissions(Domain.Enums.GroupRole.Member)
            });

            await dbContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            throw new UnauthorizedAccessException("You're not part of the group");
        }
    }
}
