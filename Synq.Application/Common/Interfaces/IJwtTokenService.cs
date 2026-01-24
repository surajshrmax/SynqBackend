using Synq.Application.DTOs.Auth;
using Synq.Domain.Entities;

namespace Synq.Application.Common.Interfaces;

public interface IJwtTokenService
{
    TokenDto GenerateNewToken(User user);
}