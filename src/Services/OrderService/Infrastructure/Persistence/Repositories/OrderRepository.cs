using Domain.Aggregates;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class OrderRepository(OrderDbContext context) : IOrderRepository
{
    private readonly OrderDbContext _context = context;

    public async Task CreateAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    public async Task<Order> GetByIdAsync(Guid orderId)
    {
        return await _context.Orders.FindAsync(orderId);
    }

    public void UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
    }

    public void DeleteAsync(Order order)
    {
        _context.Orders.Remove(order);
    }

    public async Task<List<Order>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<Order>> GetPendingOrdersAsync()
    {
        return await _context.Orders
            .Where(o => o.Status == OrderStatus.Pending)
            .ToListAsync();
    }

    public async Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status)
    {
        return await _context.Orders
            .Where(o => o.Status == status)
            .ToListAsync();
    }
}