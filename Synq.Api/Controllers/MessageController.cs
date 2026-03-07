using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Synq.Application.Features.Message.DeleteMessage;
using Synq.Application.Features.Message.GetInitialMessages;
using Synq.Application.Features.Message.GetMessagesAroundId;
using Synq.Application.Features.Message.GetNewerMessages;
using Synq.Application.Features.Message.GetOlderMessages;
using Synq.Application.Features.Message.NotifyTyping;
using Synq.Application.Features.Message.SendMessage;
using Synq.Application.Features.Message.UpdateMessage;

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

  [HttpDelete]
  public async Task<IActionResult> DeleteMessage(DeleteMessageCommand command)
  {
    await mediator.Send(command);
    return Ok();
  }

  [HttpPatch]
  public async Task<IActionResult> UpdateMessage(UpdateMessageCommand command)
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

  [HttpPost("typing")]
  public async Task<IActionResult> NotifyTyping([FromBody] NotifyTypingCommand command)
  {
    await mediator.Send(command);
    return Ok();
  }
}
