using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs;

namespace Synq.Application.Features.Group.GetMembers;

public class GetMemberHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService
) : IRequestHandler<GetMembersQuery, List<GroupMemberDto>>
{
    public async Task<List<GroupMemberDto>> Handle(GetMembersQuery request, CancellationToken cancellationToken)
    {
        var isValidRequest = await dbContext.ChatMembers.Where(cm => cm.ChatId == Guid.Parse(request.GroupId) && cm.UserId == currentUserService.UserId).FirstOrDefaultAsync(cancellationToken);
        if(isValidRequest == null)
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
        }).ToList()).FirstAsync();

        return members;
    }
}