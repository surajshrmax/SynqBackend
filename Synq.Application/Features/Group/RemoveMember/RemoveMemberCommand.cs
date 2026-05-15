using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Synq.Application.Features.Group.RemoveMember;

public record RemoveMemberCommand(string GroupId, string UserId) : IRequest;