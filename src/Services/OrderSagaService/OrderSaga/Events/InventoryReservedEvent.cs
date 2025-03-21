namespace OrderSaga.Events;

/// <summary>
/// Sau khi thanh toán thành công, hệ thống sẽ kiểm tra kho hàng. Nếu hàng còn đủ, sự kiện này sẽ được phát ra.
/// </summary>
public class InventoryReservedEvent
{
    public Guid OrderId { get; set; }
    public bool IsSuccess { get; set; }
}
