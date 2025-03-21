using System;

namespace OrderSaga.Events;

/// <summary>
/// Nếu thanh toán thất bại, hệ thống sẽ phát ra sự kiện này để hủy đơn hàng.
/// </summary>
public class OrderCanceledEvent
{
    public Guid OrderId { get; set; }
}
