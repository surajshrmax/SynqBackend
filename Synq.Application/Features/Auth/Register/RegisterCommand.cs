using MediatR;
using Synq.Application.DTOs.Auth;

namespace Synq.Application.Features.Auth.Register;

public record RegisterCommand(string Name, string Username, string Email, string Password) : IRequest<AuthResponse>;