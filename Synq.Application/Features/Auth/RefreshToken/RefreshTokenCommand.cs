using MediatR;
using Synq.Application.DTOs.Auth;

namespace Synq.Application.Features.Auth.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<TokenDto>;