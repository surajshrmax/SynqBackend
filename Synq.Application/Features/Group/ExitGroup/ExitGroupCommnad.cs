using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Synq.Application.Features.Group.ExitGroup;

public record ExitGroupCommnad(string GroupId) : IRequest;
