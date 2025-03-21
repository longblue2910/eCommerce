namespace Domain.Repositories;

using Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Repository cho Order (đơn hàng).
/// </summary>
public interface IOrderRepository
{
    Task CreateAsync(Order order);
    Task<Order> GetByIdAsync(Guid orderId);
    Task UpdateAsync(Order order);
    Task DeleteAsync(Order order);
    Task<List<Order>> GetByUserIdAsync(Guid userId);
    Task<List<Order>> GetPendingOrdersAsync();
    Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status);
}
