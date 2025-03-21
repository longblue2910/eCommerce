namespace OrderSaga.Events;

/// <summary>
/// 📌 Khi hệ thống thanh toán xử lý xong, nó sẽ phát ra sự kiện này.
/// </summary>
public class PaymentProcessedEvent
{
    public Guid OrderId { get; set; }
    public bool IsSuccess { get; set; }
}
