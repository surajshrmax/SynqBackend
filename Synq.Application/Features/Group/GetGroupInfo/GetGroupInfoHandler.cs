using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs;
using Synq.Application.Mappers;

namespace Synq.Application.Features.Group.GetGroupInfo;

public class GetGroupInfoHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService) : IRequestHandler<GetGroupInfoQuery, GroupDto>
{
    public async Task<GroupDto> Handle(GetGroupInfoQuery query, CancellationToken cancellationToken)
    {
        var isValidRequest = await dbContext.ChatMembers.AsNoTracking().Where(cm => cm.ChatId == Guid.Parse(query.Id) && cm.UserId == currentUserService.UserId).FirstOrDefaultAsync(cancellationToken);
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
        
        return chat;
    }
}