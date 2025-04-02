namespace Application.Services;

using Domain.Aggregates;
using Domain.Repositories;
using SharedKernel.Events;
using SharedKernel.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class OrderService(IOrderRepository orderRepository, RabbitMqPublisher eventPublisher) : IOrderService
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly RabbitMqPublisher _eventPublisher = eventPublisher;

    public async Task<Order> CreateOrderAsync(Guid userId, List<OrderItem> items)
    {
        var order = new Order(userId, items);
        await _orderRepository.CreateAsync(order);

        // Gửi sự kiện ra ngoài hệ thống qua RabbitMQ
        var integrationEvent = new OrderCreatedIntegrationEvent(order.Id, order.UserId, order.TotalPrice);
        await _eventPublisher.PublishAsync(integrationEvent, "order.created");
        return order;
    }
}