using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Synq.Application.Features.Message.GetInitialMessages;
using Synq.Application.Features.Message.GetMessagesAroundId;
using Synq.Application.Features.Message.GetNewerMessages;
using Synq.Application.Features.Message.GetOlderMessages;
using Synq.Application.Features.Message.SendMessage;

namespace Synq.Api.Controllers;

[Authorize]
[Route("/messages")]
[ApiController]
public class MessageController(IMediator mediator) : ControllerBase
{

  [HttpPost]
  public async Task<IActionResult> SendMessage(SendMessageCommand command)
  {
    await mediator.Send(command);
    return Ok();
  }

  [HttpGet("initial")]
  public async Task<IActionResult> GetInitialMessages([FromQuery] GetInitialMessagesQuery query)
  {
    return Ok(await mediator.Send(query));
  }

  [HttpGet("older")]
  public async Task<IActionResult> GetOlderMessages([FromQuery] GetOlderMessagesQuery query)
  {
    return Ok(await mediator.Send(query));
  }

  [HttpGet("around")]
  public async Task<IActionResult> GetAroundMessages([FromQuery] GetMessagesAroundIdQuery query)
  {
    return Ok(await mediator.Send(query));
  }

  [HttpGet("newer")]
  public async Task<IActionResult> GetNewerMessages([FromQuery] GetNewerMessagesQuery query)
  {
    return Ok(await mediator.Send(query));
  }
}
