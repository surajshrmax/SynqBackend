using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs.Auth;

namespace Synq.Application.Features.Auth.RefreshToken;

public class RefreshTokenHandler(IApplicationDbContext dbContext, IJwtTokenService jwtTokenService) : IRequestHandler<RefreshTokenCommand, TokenDto>
{
    public async Task<TokenDto> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var token = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == command.RefreshToken && !t.IsRevoked && DateTime.UtcNow < t.ExpiresAt, cancellationToken);

        if (token == null)
        {
            throw new Exception("Something went wrong can't refresh token");
        }

        token.IsRevoked = true;

        var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == token.UserId);

        var newToken = jwtTokenService.GenerateNewToken(user);
        var refreshToken = new Domain.Entities.RefreshToken(token.UserId, newToken.RefreshToken);

        await dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new TokenDto(newToken.AccessToken, newToken.RefreshToken);
    }
}
