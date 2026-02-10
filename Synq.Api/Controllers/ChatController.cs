using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Synq.Application.Features.Chats.GetAllChats;

namespace Synq.Api.Controllers;

[Route("/chats")]
[ApiController]
public class ChatController(IMediator mediator) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetChats()
    {
        var res = await mediator.Send(new GetAllChatsQuery());
        return Ok(res);
    }
    
}