using Application.Commands;
using Domain.Aggregates;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/orders")]
public class OrderController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var order = await _mediator.Send(command);
        return Ok(order);
    }
}
