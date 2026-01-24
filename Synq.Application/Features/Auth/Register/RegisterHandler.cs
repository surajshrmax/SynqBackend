using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs.Auth;
using Synq.Domain.Entities;
using Synq.Domain.ValueObjects;

namespace Synq.Application.Features.Auth.Register;

public class RegisterHandler(IApplicationDbContext dbContext, IJwtTokenService jwtTokenService, IPasswordHasher hasher) : IRequestHandler<RegisterCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var emailExists = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

        if (emailExists != null)
        {
            throw new Exception("User with this email already exists.");
        }

        var usenameExists = await dbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username.Equals(command.Username), cancellationToken);

        if (usenameExists != null)
        {
            throw new Exception("This username is already taken");
        }

        var passwordHash = PasswordHash.FromHash(hasher.HashPassword(command.Password));

        var user = new Domain.Entities.User(command.Username, command.Email, passwordHash);

        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var profile = new UserProfile(user.Id, command.Name)
        {
            LastSeenAt = DateTime.UtcNow
        };

        await dbContext.UserProfiles.AddAsync(profile, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var token = jwtTokenService.GenerateNewToken(user);

        var refreshToken = new Domain.Entities.RefreshToken(user.Id, token.RefreshToken);
        await dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return new AuthResponse(userId: user.Id,AccessToken: token.AccessToken, RefreshToken: token.RefreshToken);
    }
}