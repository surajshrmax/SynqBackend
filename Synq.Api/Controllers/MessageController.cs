using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Synq.Application.Features.Message.GetMessages;
using Synq.Application.Features.Message.SendMessage;

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
}