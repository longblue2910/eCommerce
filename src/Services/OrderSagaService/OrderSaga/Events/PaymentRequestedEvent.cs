namespace OrderSaga.Events;

/// <summary>
/// Khi nhận được OrderCreatedEvent, Saga sẽ gửi yêu cầu thanh toán.
/// </summary>
public class PaymentRequestedEvent
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
}
