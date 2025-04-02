using Application.Commands;
using Application.Services;
using Domain.Aggregates;
using MediatR;

namespace Application.Handlers;

public class CreateOrderCommandHandler(IOrderService orderService) : IRequestHandler<CreateOrderCommand, Order>
{
    private readonly IOrderService _orderService = orderService;

    public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        return await _orderService.CreateOrderAsync(request.UserId, request.Items);
    }
}
