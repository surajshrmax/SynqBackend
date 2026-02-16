using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs;

namespace Synq.Application.Features.Chats.GetAllChats;

public class GetAllChatsHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService) : IRequestHandler<GetAllChatsQuery, IEnumerable<ChatDto>>
{
  public async Task<IEnumerable<ChatDto>> Handle(GetAllChatsQuery query, CancellationToken cancellationToken)
  {
    var chats = await dbContext.Chats.AsNoTracking()
        .Where(c => c.ChatMembers.Any(m => m.UserId == currentUserService.UserId))
        .Select(
            c => new ChatDto(Id: c.Id, IsGroup: c.IsGroup, Title: c.Title,
            User: c.ChatMembers.Where(u => u.UserId != currentUserService.UserId).Select(u => new UserDto
            {
              Id = u.User.Id,
              UserProfile = new UserProfileDto
              {
                Name = u.User.UserProfile.Name,
                ImageUrl = u.User.UserProfile.ImageUrl,
              }
            }).First(),
            LastMessage: c.Messages.OrderByDescending(m => m.SentAt)
                .Select(m => new MessageDto
                {
                  Id = m.Id,
                  Content = m.Content,
                  Sender = new UserDto
                  {
                    Id = m.Sender.Id,
                    UserProfile = new UserProfileDto
                    {
                      Name = m.Sender.UserProfile.Name,
                    }
                  },
                  SenderId = m.SenderId,
                  SentAt = m.SentAt
                })
                .First()))
        .ToListAsync(cancellationToken);

    return chats;
  }
}
