using Application.Services;
using Domain.Aggregates;
using Domain.Repositories;
using MassTransit;
using SharedKernel.Events;

public class OrderService(IOrderRepository orderRepository, IBus bus) : IOrderService
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IBus _bus = bus;

    public async Task<Order> CreateOrderAsync(Guid userId, List<OrderItem> items)
    {
        var order = new Order(userId, items);
        await _orderRepository.CreateAsync(order);

        // Sử dụng MassTransit để publish message
        var integrationEvent = new OrderCreatedIntegrationEvent(order.Id, order.UserId, order.TotalPrice);

        // Cách 1: Publish trực tiếp (không cần routing key)
        await _bus.Publish(integrationEvent);

        // Cách 2: Send đến một endpoint cụ thể (nếu cần routing key)
        // var endpoint = await _bus.GetSendEndpoint(new Uri("rabbitmq://localhost/order_saga_queue"));
        // await endpoint.Send(integrationEvent);

        return order;
    }
}
