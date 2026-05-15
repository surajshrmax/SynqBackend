using MediatR;
using Synq.Application.Common.Interfaces;
using Synq.Domain.Entities;
using Synq.Domain.Permissions;

namespace Synq.Application.Features.Group.CreateGroup;

public class CreateGroupHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService
) : IRequestHandler<CreateGroupCommand>
{
  public async Task Handle(CreateGroupCommand command, CancellationToken cancellationToken)
  {
    var memebers = command.Members.ConvertAll(id => new ChatMember
    {
      UserId = Guid.Parse(id),
      Role = Domain.Enums.GroupRole.Member,
      Permissions = GroupRolePermissions.GetPermissions(Domain.Enums.GroupRole.Member)
    });

    memebers.Add(new ChatMember {
        UserId = currentUserService.UserId, 
        Role = Domain.Enums.GroupRole.Owner, 
        Permissions = GroupRolePermissions.GetPermissions(Domain.Enums.GroupRole.Owner)
    });

    var chat = new Chat
    {
      IsGroup = true,
      Title = command.Name,
      ChatMembers = memebers
    };

    await dbContext.Chats.AddAsync(chat, cancellationToken);
    await dbContext.SaveChangesAsync(cancellationToken);
  }
}
