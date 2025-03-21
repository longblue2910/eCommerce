namespace OrderSaga.Events;

/// <summary>
///  Khi hệ thống hoàn tiền xong, nó sẽ phát ra sự kiện này.
/// </summary>
public class RefundProcessedEvent
{
    public Guid OrderId { get; set; }
    public bool IsSuccess { get; set; }
}