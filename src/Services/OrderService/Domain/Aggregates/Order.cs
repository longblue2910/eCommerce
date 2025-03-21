namespace Domain.Aggregates;

using global::Domain.Events;
using SharedKernel.Common;
using System;
using System.Collections.Generic;

public class Order : AggregateRoot<Guid>
{
    public Guid UserId { get; private set; } // ID của khách hàng đặt hàng
    public List<OrderItem> Items { get; private set; } // Danh sách sản phẩm trong đơn hàng
    public decimal TotalPrice { get; private set; } // Tổng giá trị đơn hàng
    public OrderStatus Status { get; private set; } // Trạng thái đơn hàng
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow; // Ngày tạo đơn hàng
    public DateTime UpdatedAt { get; private set; } // Ngày cập nhật cuối cùng
    public bool IsRefunded { get; private set; } // Đánh dấu đơn hàng có hoàn tiền không

    private Order() : base(Guid.NewGuid()) { } // Constructor mặc định

    public Order(Guid userId, List<OrderItem> items) : base(Guid.NewGuid())
    {
        UserId = userId;
        Items = items;
        TotalPrice = CalculateTotal(items);
        Status = OrderStatus.Pending; // Mặc định đơn hàng đang chờ xử lý

        // Phát sự kiện khi đơn hàng được tạo
        AddDomainEvent(new OrderCreatedEvent(Id, UserId, TotalPrice));
    }

    /// <summary>
    /// Đánh dấu đơn hàng đang xử lý thanh toán
    /// </summary>
    public void MarkAsProcessing()
    {
        Status = OrderStatus.PaymentProcessing;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderPaymentStartedEvent(Id, TotalPrice));
    }

    /// <summary>
    /// Đánh dấu đơn hàng đã thanh toán thành công
    /// </summary>
    public void MarkAsPaid()
    {
        Status = OrderStatus.PaymentCompleted;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderPaymentCompletedEvent(Id));
    }

    /// <summary>
    /// Đánh dấu đơn hàng đã giao hàng
    /// </summary>
    public void MarkAsShipped()
    {
        Status = OrderStatus.Shipped;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderShippedEvent(Id));
    }

    /// <summary>
    /// Hủy đơn hàng
    /// </summary>
    public void MarkAsCancelled()
    {
        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderCancelledEvent(Id));
    }

    /// <summary>
    /// Yêu cầu hoàn tiền
    /// </summary>
    public void RequestRefund()
    {
        if (Status != OrderStatus.Shipped && Status != OrderStatus.Delivered)
        {
            throw new InvalidOperationException("Chỉ có thể yêu cầu hoàn tiền nếu đơn hàng đã được giao.");
        }

        IsRefunded = true;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderRefundRequestedEvent(Id, TotalPrice));
    }

    /// <summary>
    /// Hoàn tất hoàn tiền
    /// </summary>
    public void CompleteRefund()
    {
        if (!IsRefunded)
        {
            throw new InvalidOperationException("Không thể hoàn tiền nếu đơn hàng chưa có yêu cầu hoàn trả.");
        }

        Status = OrderStatus.Refunded;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderRefundCompletedEvent(Id));
    }

    /// <summary>
    /// Tính tổng giá trị đơn hàng
    /// </summary>
    private decimal CalculateTotal(List<OrderItem> items)
    {
        return items.Sum(item => item.TotalPrice);
    }
}

/// <summary>
/// Các trạng thái đơn hàng
/// </summary>
public enum OrderStatus
{
    Pending,
    PaymentProcessing,
    PaymentCompleted,
    Shipped,
    Delivered,
    Cancelled,
    Refunded
}
