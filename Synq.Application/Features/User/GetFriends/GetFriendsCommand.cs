using MediatR;
using Synq.Application.DTOs.User;

namespace Synq.Application.Features.User.GetFriends;

public record GetFriendsCommand(string? Keyword = null) : IRequest<GetFriendsResponse>;
