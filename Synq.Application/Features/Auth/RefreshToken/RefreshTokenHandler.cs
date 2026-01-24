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
            .Where(t => t.Token == command.RefreshToken && t.ExpiresAt < DateTime.UtcNow && !t.IsRevoked)
            .Include(t => t.User)
            .FirstOrDefaultAsync(cancellationToken);

        if (token == null)
        {
            throw new Exception("Something went wrong can't refresh token");
        }

        token.IsRevoked = true;

        var newToken = jwtTokenService.GenerateNewToken(token.User);
        var refreshToken = new Domain.Entities.RefreshToken(token.UserId, newToken.RefreshToken);

        await dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new TokenDto(newToken.AccessToken, newToken.RefreshToken);
    }
}