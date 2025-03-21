using OrderSaga.Events;
using OrderSaga.Messaging;

namespace OrderSaga;

public class OrderSagaOrchestrator
{
    private readonly IMessageBus _messageBus;

    public OrderSagaOrchestrator(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task StartSagaAsync(OrderCreatedEvent orderEvent)
    {
        Console.WriteLine($"[OrderSaga] Đơn hàng {orderEvent.OrderId} được tạo. Yêu cầu thanh toán...");
        await _messageBus.PublishAsync("payment.requested", new PaymentRequestedEvent
        {
            OrderId = orderEvent.OrderId,
            Amount = orderEvent.TotalPrice
        });
    }

    public async Task HandlePaymentProcessedAsync(PaymentProcessedEvent paymentEvent)
    {
        if (paymentEvent.IsSuccess)
        {
            Console.WriteLine($"[OrderSaga] Thanh toán thành công. Kiểm tra kho...");
            await _messageBus.PublishAsync("inventory.reserved", new InventoryReservedEvent
            {
                OrderId = paymentEvent.OrderId,
                IsSuccess = true
            });
        }
        else
        {
            Console.WriteLine($"[OrderSaga] Thanh toán thất bại. Hủy đơn hàng...");
            await _messageBus.PublishAsync("order.canceled", new OrderCanceledEvent
            {
                OrderId = paymentEvent.OrderId
            });
        }
    }

    public async Task HandleInventoryReservedAsync(InventoryReservedEvent inventoryEvent)
    {
        if (inventoryEvent.IsSuccess)
        {
            Console.WriteLine($"[OrderSaga] Kho hàng đã giữ thành công. Xác nhận đơn hàng...");
            await _messageBus.PublishAsync("order.confirmed", new OrderConfirmedEvent
            {
                OrderId = inventoryEvent.OrderId
            });
        }
        else
        {
            Console.WriteLine($"[OrderSaga] Kho hết hàng. Hoàn tiền...");
            await _messageBus.PublishAsync("refund.requested", new RefundRequestedEvent
            {
                OrderId = inventoryEvent.OrderId
            });
        }
    }
}
