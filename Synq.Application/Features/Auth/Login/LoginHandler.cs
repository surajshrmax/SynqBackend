using MediatR;
using Microsoft.EntityFrameworkCore;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs.Auth;
using Synq.Application.Exceptions;

namespace Synq.Application.Features.Auth.Login;

public class LoginHandler(IApplicationDbContext dbContext, IPasswordHasher hasher, IJwtTokenService jwtTokenService) : IRequestHandler<LoginCommand, AuthResponse>
{
  public async Task<AuthResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
  {
    var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

    if (user == null)
    {
      throw new AuthInvalidCredentialsException("Either email or password is wrong");
    }

    if (!hasher.VerifyPassword(user.PasswordHash.Value, command.Password))
    {
      throw new AuthInvalidCredentialsException("Either email or password is wrong");
    }

    var token = jwtTokenService.GenerateNewToken(user);

    var refreshToken = new Domain.Entities.RefreshToken(user.Id, token.RefreshToken);
    await dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    await dbContext.SaveChangesAsync(cancellationToken);

    return new AuthResponse(user.Id, token.AccessToken, token.RefreshToken);
  }
}
