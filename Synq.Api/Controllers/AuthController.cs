using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Synq.Application.Features.Auth.Login;
using Synq.Application.Features.Auth.Logout;
using Synq.Application.Features.Auth.RefreshToken;
using Synq.Application.Features.Auth.Register;

namespace Synq.Api.Controllers;

[Route("/auth")]
[ApiController]
public class AuthController(IMediator mediator) : ControllerBase
{

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(RegisterCommand registerCommand)
    {
        var res = await mediator.Send(registerCommand);
        return Ok(res);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser(LoginCommand command)
    {
        var res = await mediator.Send(command);
        return Ok(res);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenCommand command)
    {
        var res = await mediator.Send(command);
        return Ok(res);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutUser()
    {
        var res = await mediator.Send(new LogoutCommand());
        return res ? Ok() : Forbid();
    }
}