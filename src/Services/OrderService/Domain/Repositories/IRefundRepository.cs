namespace Domain.Repositories;

using Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Repository cho Refund (hoàn tiền).
/// </summary>
public interface IRefundRepository
{
    Task CreateAsync(Refund refund);
    Task<Refund> GetByIdAsync(Guid refundId);
    Task<Refund> GetByOrderIdAsync(Guid orderId);
    void UpdateAsync(Refund refund);
    void DeleteAsync(Refund refund);
    Task<List<Refund>> GetPendingRefundsAsync();
    Task<List<Refund>> GetApprovedRefundsAsync();
    Task<List<Refund>> GetRejectedRefundsAsync();
}
