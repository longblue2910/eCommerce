namespace OrderSaga.Events;

/// <summary>
/// Nếu kho hết hàng, hệ thống sẽ phát sự kiện này để hoàn tiền và hủy đơn hàng.
/// </summary>
public class InventoryFailedEvent
{
    public Guid OrderId { get; set; }
}
