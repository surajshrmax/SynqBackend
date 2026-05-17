using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs;

namespace Synq.Application.Features.Group.GetMembers;

public class GetMemberHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    ICacheService cacheService,
    IJsonHelper<List<GroupMemberDto>> jsonHelper
) : IRequestHandler<GetMembersQuery, List<GroupMemberDto>>
{
  public async Task<List<GroupMemberDto>> Handle(GetMembersQuery request, CancellationToken cancellationToken)
  {

    string key = $"get-members {request.GroupId}";

    var cached = await cacheService.GetValueAsync(key);

    if (cached != null)
    {
      return jsonHelper.Decode(cached);
    }

    var isValidRequest = await dbContext.ChatMembers.Where(cm => cm.ChatId == Guid.Parse(request.GroupId) && cm.UserId == currentUserService.UserId).FirstOrDefaultAsync(cancellationToken);
    if (isValidRequest == null)
    {
      throw new UnauthorizedAccessException("You're not part of the group.");
    }

    var members = await dbContext.Chats.AsNoTracking().Where(c => c.Id == Guid.Parse(request.GroupId)).Select(c => c.ChatMembers.Select(cm => new GroupMemberDto
    {
      Id = cm.UserId,
      Role = cm.Role.ToString(),
      Name = cm.User.UserProfile.Name,
      ImageUrl = cm.User.UserProfile.ImageUrl,
      JoinedDate = cm.CreatedAt
    }).ToList()).FirstAsync(cancellationToken);

    await cacheService.SetValueAsync(key, jsonHelper.Encode(members), TimeSpan.FromMinutes(60));

    return members;
  }
}
