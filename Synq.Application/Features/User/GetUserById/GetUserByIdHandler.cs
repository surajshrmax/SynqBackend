using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs;
using Synq.Application.Mappers;

namespace Synq.Application.Features.User.GetUserById;

public class GetUserByIdHandler(IApplicationDbContext dbContext) : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    public async Task<UserDto?> Handle(GetUserByIdQuery command, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.AsNoTracking()
            .Where(u => u.Id == command.UserId)
            .Include(u => u.UserProfile)
            .FirstAsync(cancellationToken);

        return user.ToDto();
    }
}