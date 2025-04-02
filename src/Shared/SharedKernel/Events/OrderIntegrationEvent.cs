namespace SharedKernel.Events;


public class OrderCreatedIntegrationEvent(Guid orderId, Guid userId, decimal totalPrice) : IntegrationEvent
{
    public Guid OrderId { get; private set; } = orderId;
    public Guid UserId { get; private set; } = userId;
    public decimal TotalPrice { get; private set; } = totalPrice;
}

public class OrderPaymentStartedIntegrationEvent(Guid orderId, decimal amount) : IntegrationEvent
{
    public Guid OrderId { get; private set; } = orderId;
    public decimal Amount { get; private set; } = amount;
}

public class OrderPaymentCompletedIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; private set; }

    public OrderPaymentCompletedIntegrationEvent(Guid orderId) => OrderId = orderId;
}

public class OrderShippedIntegrationEvent(Guid orderId, string trackingNumber) : IntegrationEvent
{
    public Guid OrderId { get; private set; } = orderId;
    public string TrackingNumber { get; private set; } = trackingNumber;
}

public class OrderDeliveredIntegrationEvent(Guid orderId) : IntegrationEvent
{
    public Guid OrderId { get; private set; } = orderId;
}

public class OrderCancelledIntegrationEvent(Guid orderId, string reason) : IntegrationEvent
{
    public Guid OrderId { get; private set; } = orderId;
    public string Reason { get; private set; } = reason;
}

public class OrderRefundRequestedIntegrationEvent(Guid orderId, decimal refundAmount) : IntegrationEvent
{
    public Guid OrderId { get; private set; } = orderId;
    public decimal RefundAmount { get; private set; } = refundAmount;
}

public class OrderRefundCompletedIntegrationEvent(Guid orderId) : IntegrationEvent
{
    public Guid OrderId { get; private set; } = orderId;
}
