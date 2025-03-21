namespace Domain.Repositories;

using System;
using System.Threading.Tasks;

/// <summary>
/// Định nghĩa Unit of Work để xử lý transaction.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IOrderRepository Orders { get; }
    IPaymentRepository Payments { get; }
    IRefundRepository Refunds { get; }
    Task<int> SaveChangesAsync();
}