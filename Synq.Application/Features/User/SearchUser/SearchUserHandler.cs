using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs;

namespace Synq.Application.Features.User.SearchUser;

public class SearchUserHandler(IApplicationDbContext dbContext) : IRequestHandler<SearchUserQuery, IEnumerable<UserDto>>
{
  public async Task<IEnumerable<UserDto>> Handle(SearchUserQuery command, CancellationToken cancellationToken)
  {
    return await dbContext.Users
        .AsNoTracking()
        .Where(u => u.Username.Contains(command.Query))
        .Select(u => new UserDto
        {
          Id = u.Id,
          Email = u.Email,
          Username = u.Username,
          UserProfile = new UserProfileDto
          {
            Name = u.UserProfile.Name,
            Bio = u.UserProfile.Bio,
            ImageUrl = u.UserProfile.ImageUrl,
            LastSeenAt = u.UserProfile.LastSeenAt
          }
        })
        .ToListAsync(cancellationToken);
  }
}
