using MediatR;
using Synq.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Synq.Application.Features.Group.GetMembers;

public record GetMembersQuery(string GroupId) : IRequest<List<GroupMemberDto>>;