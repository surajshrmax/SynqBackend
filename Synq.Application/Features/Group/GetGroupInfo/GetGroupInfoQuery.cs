
using MediatR;
using Synq.Application.DTOs;

namespace Synq.Application.Features.Group.GetGroupInfo;

public record GetGroupInfoQuery(string Id) : IRequest<GroupDto>;