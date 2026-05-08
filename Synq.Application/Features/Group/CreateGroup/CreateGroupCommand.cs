using MediatR;

namespace Synq.Application.Features.Group.CreateGroup;

public record CreateGroupCommand(string Name, string ImageUrl, List<string> Members) : IRequest;

