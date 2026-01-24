using MediatR;
using Synq.Application.DTOs;

namespace Synq.Application.Features.User.SearchUser;

public record SearchUserQuery(string Query) : IRequest<IEnumerable<UserDto>>;