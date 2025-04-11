namespace OrderSaga.Worker.Entities;

/// <summary>
/// Lưu trữ trạng thái của Saga xử lý đơn hàng
/// </summary>
public class OrderSagaState
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public SagaStatus Status { get; set; }
    public string FailureReason { get; set; }
    public OrderSagaStep CurrentStep { get; set; }
    public string PaymentTransactionId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public enum SagaStatus
{
    Started,
    Completed,
    Failed,
    CompensationFailed
}

public enum OrderSagaStep
{
    NotStarted,
    OrderMarkedAsProcessing,
    InventoryReserved,
    PaymentProcessed,
    OrderCompleted
}

/// <summary>
/// DTO cho thông tin đơn hàng
/// </summary>
public class OrderDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
