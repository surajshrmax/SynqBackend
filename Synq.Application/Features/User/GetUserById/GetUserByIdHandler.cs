using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs;

namespace Synq.Application.Features.User.GetUserById;

public class GetUserByIdHandler(IApplicationDbContext dbContext) : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    public async Task<UserDto?> Handle(GetUserByIdQuery command, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.AsNoTracking()
            .Where(u => u.Id == command.UserId)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                UserProfile = new UserProfileDto
                {
                    Name = u.UserProfile.Name,
                    Bio = u.UserProfile.Bio,
                    ImageUrl = u.UserProfile.ImageUrl,
                    LastSeenAt = u.UserProfile.LastSeenAt
                }
            })
            .FirstOrDefaultAsync(cancellationToken);

        return user;
    }
}