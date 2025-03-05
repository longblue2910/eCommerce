using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var commandWithIp = command with { IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() };
        var result = await _mediator.Send(commandWithIp);

        var response = await _mediator.Send(command);
        return Ok(response);
    }
}