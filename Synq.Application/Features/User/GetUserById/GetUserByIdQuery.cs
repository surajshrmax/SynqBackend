using MediatR;
using Synq.Application.DTOs;

namespace Synq.Application.Features.User.GetUserById;

public record GetUserByIdQuery(Guid UserId) : IRequest<UserDto?>;