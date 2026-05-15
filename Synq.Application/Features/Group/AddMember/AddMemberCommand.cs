using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Synq.Application.Features.Group.AddMember;
public record AddMemberCommand(string GroupId, string UserId) : IRequest;