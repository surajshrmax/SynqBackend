using MediatR;

namespace Synq.Application.Features.Auth.Logout;

public record LogoutCommand : IRequest<bool>;