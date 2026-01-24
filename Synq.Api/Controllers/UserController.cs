using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Synq.Application.Features.User.GetUserById;
using Synq.Application.Features.User.SearchUser;

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

    [Authorize]
    [HttpGet("search/{search}")]
    public async Task<IActionResult> SearchUser(string search)
    {
        var searchUserQuery = new SearchUserQuery(search);
        var users = await mediator.Send(searchUserQuery);

        return Ok(users);
    }

    [Authorize]
    [HttpGet]
    public IActionResult GetProfile()
    {
        return Ok(User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}