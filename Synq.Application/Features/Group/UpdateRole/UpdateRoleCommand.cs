using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Synq.Application.Features.Group.UpdateRole;

public record UpdateRoleCommand(string GroupId, string UserId, string Role) : IRequest;
