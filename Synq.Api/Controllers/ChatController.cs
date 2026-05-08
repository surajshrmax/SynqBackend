using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Synq.Application.Features.Chats.GetAllChats;
using Synq.Application.Features.Group.CreateGroup;
using Synq.Application.Features.Group.GetGroupInfo;

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
  
}
