using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs;

namespace Synq.Application.Features.Group.GetGroupInfo;

public class GetGroupInfoHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    ICacheService cacheService,
    IJsonHelper<GroupDto> jsonHelper) : IRequestHandler<GetGroupInfoQuery, GroupDto>
{
  public async Task<GroupDto> Handle(GetGroupInfoQuery query, CancellationToken cancellationToken)
  {
    string key = $"group-info {query.Id}";

    string cachedGroup = await cacheService.GetValueAsync(key);

    if (cachedGroup != null)
    {
      return jsonHelper.Decode(cachedGroup);
    }

    var isValidRequest = await dbContext.ChatMembers
        .AsNoTracking()
        .Where(cm => cm.ChatId == Guid.Parse(query.Id) &&
            cm.UserId == currentUserService.UserId)
        .FirstOrDefaultAsync(cancellationToken);

    if (isValidRequest == null)
    {
      throw new UnauthorizedAccessException("You're not part of the group");
    }

    var chat = await dbContext.Chats.AsNoTracking().Where(c => c.Id == Guid.Parse(query.Id))
        .Select(c => new GroupDto
        {
          Id = c.Id,
          Title = c.Title,
          ImageUrl = "",
          MembersCount = c.ChatMembers.Count
        }).FirstAsync(cancellationToken);

    if (chat != null)
    {
      await cacheService.SetValueAsync(key, jsonHelper.Encode(chat), TimeSpan.FromMinutes(10));
    }

    return chat;
  }
}
