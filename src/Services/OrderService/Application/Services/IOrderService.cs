using Domain.Aggregates;

namespace Application.Services;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(Guid userId, List<OrderItem> items);
}