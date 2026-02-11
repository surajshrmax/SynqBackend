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
    [HttpGet]
    public async Task<IActionResult> GetMessages([FromQuery] GetMessagesQuery query)
    {
        return Ok(await mediator.Send(query));
    }
}
