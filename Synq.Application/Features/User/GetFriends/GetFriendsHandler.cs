using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs;
using Synq.Application.DTOs.User;
using Synq.Application.Mappers;

namespace Synq.Application.Features.User.GetFriends;

public class GetFriendsHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService
) : IRequestHandler<GetFriendsCommand, GetFriendsResponse>
{
  readonly int pageLimit = 20;
  public async Task<GetFriendsResponse> Handle(GetFriendsCommand request, CancellationToken cancellationToken)
  {
    List<UserDto> friends;
    var query =  dbContext.Chats.AsNoTracking();
    if (request.Keyword != null)
    {
      query = query.Where(c => !c.IsGroup &&
                         c.ChatMembers.Any(cm => cm.UserId == currentUserService.UserId) && c.ChatMembers.Any(c => c.UserId != currentUserService.UserId && c.User.Username.Contains(request.Keyword))
      );
    }
    else
    {
      query = query.Where(c => !c.IsGroup &&
                         c.ChatMembers.Any(cm => cm.UserId == currentUserService.UserId)
      );
    }
      
      friends = await query.Select(c => c.ChatMembers.Where(cm => cm.UserId != currentUserService.UserId).Select(u => new UserDto
      {
        Id = u.UserId,
        Username = u.User.Username,
        UserProfile = new UserProfileDto
        {
          Name = u.User.UserProfile.Name,
          ImageUrl = u.User.UserProfile.ImageUrl
        }
      }).FirstOrDefault())
      .ToListAsync(cancellationToken);

    return new GetFriendsResponse
    {
      Friends = friends ?? []
    };
  }
}
