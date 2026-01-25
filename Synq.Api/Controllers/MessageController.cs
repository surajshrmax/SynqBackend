using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Synq.Application.Features.Message.DeleteMessage;
using Synq.Application.Features.Message.GetMessages;
using Synq.Application.Features.Message.SendMessage;
using Synq.Application.Features.Message.UpdateMessage;

namespace Synq.Api.Controllers;

[Route("/messages")]
[ApiController]
public class MessageController(IMediator mediator) : ControllerBase
{

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> SendMessage(SendMessageCommand command)
    {
        var id = await mediator.Send(command);
        return Ok(id);
    }

    [Authorize]
    [HttpGet("{chatId}")]
    public async Task<IActionResult> GetMessages(string chatId)
    {
        return Ok(await mediator.Send(new GetMessagesQuery(chatId)));
    }

    [Authorize]
    [HttpPatch]
    public async Task<IActionResult> UpdateMessage(UpdateMessageCommand command)
    {
        var res = await mediator.Send(command);
        if (res == null)
        {
            return Unauthorized();
        }
        return Ok(res);
    }

    [Authorize]
    [HttpDelete("{messageId}")]
    public async Task<IActionResult> DeleteMessage(string messageId)
    {
        var res = await mediator.Send(new DeleteMessageCommand(messageId));
        if (res)
        {
            return NoContent();
        }

        return Forbid();
    }
    
}