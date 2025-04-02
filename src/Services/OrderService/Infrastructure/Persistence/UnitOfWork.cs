namespace Infrastructure.Persistence;

using Domain.Repositories;

public class UnitOfWork(OrderDbContext context, IOrderRepository orders, IPaymentRepository payments, IRefundRepository refunds) : IUnitOfWork
{
    private readonly OrderDbContext _context = context;
    public IOrderRepository Orders { get; } = orders;
    public IPaymentRepository Payments { get; } = payments;
    public IRefundRepository Refunds { get; } = refunds;

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
