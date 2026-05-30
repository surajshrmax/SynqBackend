using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Synq.Application.Features.Chats.GetAllChats;
using Synq.Application.Features.Group.AddMember;
using Synq.Application.Features.Group.CreateGroup;
using Synq.Application.Features.Group.ExitGroup;
using Synq.Application.Features.Group.GetGroupInfo;
using Synq.Application.Features.Group.GetMembers;
using Synq.Application.Features.Group.RemoveMember;
using Synq.Application.Features.Group.UpdateRole;

namespace Synq.Api.Controllers;

[Authorize]
[Route("/chats")]
[ApiController]
public class ChatController(IMediator mediator) : ControllerBase
{
  [HttpGet]
  public async Task<IActionResult> GetChats()
  {
    var res = await mediator.Send(new GetAllChatsQuery());
    return Ok(res);
  }

  [HttpPost]
  public async Task<IActionResult> CreateGroup([FromBody] CreateGroupCommand command)
  {
    await mediator.Send(command);
    return Ok();
  }

  [HttpGet("group")]
  public async Task<ActionResult> GetGroupInfo([FromQuery] GetGroupInfoQuery query)
  {
    return Ok(await mediator.Send(query));
  }

  [HttpGet("group/members")]
  public async Task<ActionResult> GetGroupMembers([FromQuery] GetMembersQuery query)
  {
    return Ok(await mediator.Send(query));
  }

  [HttpPost("group")]
  public async Task<IActionResult> AddMember([FromBody] AddMemberCommand command)
  {
    await mediator.Send(command);
    return Ok();
  }

  [HttpDelete("group/members")]
  public async Task<IActionResult> RemoveMember([FromBody] RemoveMemberCommand command)
  {
    await mediator.Send(command);
    return Ok();
  }

  [HttpPost("group/exit")]
  public async Task<IActionResult> ExitGroup([FromQuery] ExitGroupCommnad commnad)
  {
    await mediator.Send(commnad);
    return Ok();
  }

  [HttpPatch("group/member")]
  public async Task<IActionResult> UpdateMemberRole([FromBody] UpdateRoleCommand command)
  {
    await mediator.Send(command);
    return Ok();
  }

}
