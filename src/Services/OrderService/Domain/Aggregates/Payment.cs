﻿namespace Domain.Aggregates;

using Domain.Events;
using SharedKernel.Common;
using System;

public class Payment : AggregateRoot<Guid>
{
    public Guid OrderId { get; private set; } // ID của đơn hàng
    public decimal Amount { get; private set; } // Số tiền thanh toán
    public PaymentStatus Status { get; private set; } // Trạng thái thanh toán
    public PaymentMethod Method { get; private set; } // Phương thức thanh toán
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow; // Ngày tạo thanh toán

    private Payment() : base(Guid.NewGuid()) { }

    public Payment(Guid orderId, decimal amount, PaymentMethod method) : base(Guid.NewGuid())
    {
        OrderId = orderId;
        Amount = amount;
        Method = method;
        Status = PaymentStatus.Pending;

        AddDomainEvent(new PaymentCreatedEvent(Id, OrderId, Amount));
    }

    public void MarkAsCompleted()
    {
        Status = PaymentStatus.Completed;
        AddDomainEvent(new PaymentCompletedEvent(Id, OrderId));
    }

    public void MarkAsFailed()
    {
        Status = PaymentStatus.Failed;
        AddDomainEvent(new PaymentFailedEvent(Id, OrderId));
    }
}

/// <summary>
/// Trạng thái thanh toán
/// </summary>
public enum PaymentStatus
{
    Pending,
    Completed,
    Failed
}

/// <summary>
/// Phương thức thanh toán
/// </summary>
public enum PaymentMethod
{
    MoMo,
    BankTransfer,
    VNPay,
    Cash // Tiền mặt
}

