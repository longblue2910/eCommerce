namespace Domain.Repositories;

using Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Repository cho Payment (thanh toán).
/// </summary>
public interface IPaymentRepository
{
    Task CreateAsync(Payment payment);
    Task<Payment> GetByIdAsync(Guid paymentId);
    Task<Payment> GetByOrderIdAsync(Guid orderId);
    void UpdateAsync(Payment payment);
    void DeleteAsync(Payment payment);
    Task<List<Payment>> GetFailedPaymentsAsync();
    Task<List<Payment>> GetCompletedPaymentsAsync();
}
