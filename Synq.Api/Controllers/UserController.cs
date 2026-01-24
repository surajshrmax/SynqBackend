using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Synq.Application.Features.User.GetUserById;

namespace Synq.Api.Controllers;

[Route("/users")]
[ApiController]
public class UserController(IMediator mediator) : ControllerBase
{
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var userId = Guid.Parse(id);
        var getUserByIdQuery = new GetUserByIdQuery(userId);
        var res =await mediator.Send(getUserByIdQuery);
        if (res == null)
        {
            return NotFound();
        }

        return Ok(res);
    }
    
}