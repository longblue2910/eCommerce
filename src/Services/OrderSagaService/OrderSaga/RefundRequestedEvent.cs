namespace OrderSaga.Events;

/// <summary>
/// Nếu đơn hàng bị hủy sau khi thanh toán, hệ thống sẽ gửi yêu cầu hoàn tiền.
/// </summary>
public class RefundRequestedEvent
{
    public Guid OrderId { get; set; }
}
