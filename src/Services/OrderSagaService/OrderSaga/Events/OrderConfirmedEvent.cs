using System;

namespace OrderSaga.Events;

/// <summary>
/// Khi thanh toán thành công, hệ thống sẽ gửi sự kiện này để xác nhận đơn hàng.
/// </summary>
public class OrderConfirmedEvent
{
    public Guid OrderId { get; set; }
}
