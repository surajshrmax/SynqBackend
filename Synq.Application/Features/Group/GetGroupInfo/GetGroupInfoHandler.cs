using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs;
using Synq.Application.Mappers;

namespace Synq.Application.Features.Group.GetGroupInfo;

public class GetGroupInfoHandler(IApplicationDbContext dbContext) : IRequestHandler<GetGroupInfoQuery, GroupDto>
{
    public async Task<GroupDto> Handle(GetGroupInfoQuery query, CancellationToken cancellationToken)
    {
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