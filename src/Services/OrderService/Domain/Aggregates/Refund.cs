namespace Domain.Aggregates;

using Domain.Events;
using SharedKernel.Common;
using System;

public class Refund : AggregateRoot<Guid>
{
    public Guid OrderId { get; private set; } // ID đơn hàng được hoàn tiền
    public decimal Amount { get; private set; } // Số tiền hoàn trả
    public RefundStatus Status { get; private set; } // Trạng thái hoàn tiền
    public DateTime RequestedAt { get; private set; } = DateTime.UtcNow; // Ngày yêu cầu hoàn tiền

    private Refund() : base(Guid.NewGuid()) { }

    public Refund(Guid orderId, decimal amount) : base(Guid.NewGuid())
    {
        OrderId = orderId;
        Amount = amount;
        Status = RefundStatus.Requested;

        AddDomainEvent(new RefundRequestedEvent(Id, OrderId, Amount));
    }

    public void Approve()
    {
        Status = RefundStatus.Approved;
        AddDomainEvent(new RefundApprovedEvent(Id, OrderId));
    }

    public void Reject()
    {
        Status = RefundStatus.Rejected;
        AddDomainEvent(new RefundRejectedEvent(Id, OrderId));
    }
}

/// <summary>
/// Trạng thái hoàn tiền
/// </summary>
public enum RefundStatus
{
    Requested,
    Approved,
    Rejected
}
