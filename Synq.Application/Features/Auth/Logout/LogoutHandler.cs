using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;

namespace Synq.Application.Features.Auth.Logout;

public class LogoutHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService) : IRequestHandler<LogoutCommand, bool>
{
    public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == currentUserService.UserId, cancellationToken);

        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var token = await dbContext.RefreshTokens.FirstOrDefaultAsync(t =>
            t.UserId == user.Id && t.ExpiresAt < DateTime.UtcNow, cancellationToken);

        if (token == null) return false;
        
        token.IsRevoked = true;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}