using MediatR;
using Synq.Application.Common.Interfaces;
using Synq.Domain.Entities;

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
      UserId = Guid.Parse(id)
    });

    memebers.Add(new ChatMember { IsAdmin = true, UserId = currentUserService.UserId });

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
