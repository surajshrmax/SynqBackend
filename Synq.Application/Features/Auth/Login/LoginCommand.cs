using MediatR;
using Synq.Application.DTOs.Auth;

namespace Synq.Application.Features.Auth.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;