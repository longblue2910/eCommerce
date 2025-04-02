namespace Infrastructure.Persistence.Repositories;

using Domain.Aggregates;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

public class PaymentRepository(OrderDbContext context) : IPaymentRepository
{
    private readonly OrderDbContext _context = context;

    public async Task CreateAsync(Payment payment)
    {
        await _context.Payments.AddAsync(payment);
    }

    public async Task<Payment> GetByIdAsync(Guid paymentId)
    {
        return await _context.Payments.FindAsync(paymentId);
    }

    public async Task<Payment> GetByOrderIdAsync(Guid orderId)
    {
        return await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
    }

    public void UpdateAsync(Payment payment)
    {
        _context.Payments.Update(payment);
    }

    public void DeleteAsync(Payment payment)
    {
        _context.Payments.Remove(payment);
    }

    public async Task<List<Payment>> GetFailedPaymentsAsync()
    {
        return await _context.Payments
            .Where(p => p.Status == PaymentStatus.Failed)
            .ToListAsync();
    }

    public async Task<List<Payment>> GetCompletedPaymentsAsync()
    {
        return await _context.Payments
            .Where(p => p.Status == PaymentStatus.Completed)
            .ToListAsync();
    }
}
