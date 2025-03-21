using OrderSaga.Events;
using OrderSaga.Messaging;

namespace OrderSaga;

//public class OrderSagaWorker(IMessageBus messageBus, OrderSagaOrchestrator orchestrator) : BackgroundService
//{
//    private readonly IMessageBus _messageBus = messageBus;
//    private readonly OrderSagaOrchestrator _orchestrator = orchestrator;

//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        Console.WriteLine("[OrderSagaWorker] Worker đang chạy...");

//        // Lắng nghe khi đơn hàng được tạo
//        _messageBus.Subscribe<OrderCreatedEvent>("order.created", async (orderEvent) =>
//        {
//            await _orchestrator.StartSagaAsync(orderEvent);
//        });

//        // Lắng nghe kết quả thanh toán
//        _messageBus.Subscribe<PaymentProcessedEvent>("payment.processed", async (paymentEvent) =>
//        {
//            await _orchestrator.HandlePaymentProcessedAsync(paymentEvent);
//        });

//        // Lắng nghe kết quả kiểm tra kho
//        _messageBus.Subscribe<InventoryReservedEvent>("inventory.reserved", async (inventoryEvent) =>
//        {
//            await _orchestrator.HandleInventoryReservedAsync(inventoryEvent);
//        });

//        await Task.CompletedTask;
//    }
//}
