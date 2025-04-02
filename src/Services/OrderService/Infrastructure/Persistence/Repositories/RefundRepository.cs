namespace Infrastructure.Persistence.Repositories;

using Domain.Aggregates;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

public class RefundRepository(OrderDbContext context) : IRefundRepository
{
    private readonly OrderDbContext _context = context;

    public async Task CreateAsync(Refund refund)
    {
        await _context.Refunds.AddAsync(refund);
    }

    public async Task<Refund> GetByIdAsync(Guid refundId)
    {
        return await _context.Refunds.FindAsync(refundId);
    }

    public async Task<Refund> GetByOrderIdAsync(Guid orderId)
    {
        return await _context.Refunds.FirstOrDefaultAsync(r => r.OrderId == orderId);
    }

    public void UpdateAsync(Refund refund)
    {
        _context.Refunds.Update(refund);
    }

    public void DeleteAsync(Refund refund)
    {
        _context.Refunds.Remove(refund);
    }

    public async Task<List<Refund>> GetPendingRefundsAsync()
    {
        return await _context.Refunds
            .Where(r => r.Status == RefundStatus.Requested)
            .ToListAsync();
    }

    public async Task<List<Refund>> GetApprovedRefundsAsync()
    {
        return await _context.Refunds
            .Where(r => r.Status == RefundStatus.Approved)
            .ToListAsync();
    }

    public async Task<List<Refund>> GetRejectedRefundsAsync()
    {
        return await _context.Refunds
            .Where(r => r.Status == RefundStatus.Rejected)
            .ToListAsync();
    }
}
