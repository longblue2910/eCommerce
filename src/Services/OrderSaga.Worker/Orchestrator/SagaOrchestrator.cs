// src/Services/OrderSaga.Worker/Orchestrator/SagaOrchestrator.cs
using OrderSaga.Worker.Entities;
using OrderSaga.Worker.Repositories;
using OrderSaga.Worker.Services;
using OrderSaga.Worker.Services.Implementations;
using SharedKernel.Events;

namespace OrderSaga.Worker.Orchestrator;

public class SagaOrchestrator : ISagaOrchestrator
{
    private readonly ILogger<SagaOrchestrator> _logger;
    private readonly IOrderServiceClient _orderService;
    private readonly IInventoryServiceClient _inventoryService;
    private readonly IPaymentServiceClient _paymentService;
    private readonly INotificationServiceClient _notificationService;
    private readonly IServiceProvider _serviceProvider;

    public SagaOrchestrator(
        ILogger<SagaOrchestrator> logger,
        IOrderServiceClient orderService,
        IInventoryServiceClient inventoryService,
        IPaymentServiceClient paymentService,
        INotificationServiceClient notificationService,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _orderService = orderService;
        _inventoryService = inventoryService;
        _paymentService = paymentService;
        _notificationService = notificationService;
        _serviceProvider = serviceProvider;
    }

    public async Task StartOrderProcessingSaga(OrderCreatedIntegrationEvent orderEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting order processing saga for order: {OrderId}", orderEvent.OrderId);

        using var scope = _serviceProvider.CreateScope();
        var stateRepository = scope.ServiceProvider.GetRequiredService<ISagaStateRepository>();

        // Khởi tạo trạng thái Saga
        var sagaState = new OrderSagaState
        {
            OrderId = orderEvent.OrderId,
            UserId = orderEvent.UserId,
            TotalAmount = orderEvent.TotalPrice,
            Status = SagaStatus.Started,
            StartedAt = DateTime.UtcNow
        };

        await stateRepository.SaveSagaStateAsync(sagaState, cancellationToken);

        try
        {
            // Bước 1: Đánh dấu đơn hàng đang xử lý thanh toán
            _logger.LogInformation("Marking order as processing: {OrderId}", orderEvent.OrderId);
            await _orderService.MarkOrderAsProcessingAsync(orderEvent.OrderId, cancellationToken);
            sagaState.CurrentStep = OrderSagaStep.OrderMarkedAsProcessing;
            await stateRepository.UpdateSagaStateAsync(sagaState, cancellationToken);

            // Bước 2: Kiểm tra và đặt hàng trong kho
            _logger.LogInformation("Reserving inventory for order: {OrderId}", orderEvent.OrderId);
            var inventoryReserved = await _inventoryService.ReserveInventoryAsync(orderEvent.OrderId, cancellationToken);

            if (!inventoryReserved)
            {
                _logger.LogWarning("Inventory reservation failed for order: {OrderId}", orderEvent.OrderId);
                await CompensateAndCancelOrderAsync(sagaState, "Không đủ hàng trong kho", cancellationToken);
                return;
            }

            sagaState.CurrentStep = OrderSagaStep.InventoryReserved;
            await stateRepository.UpdateSagaStateAsync(sagaState, cancellationToken);

            // Bước 3: Xử lý thanh toán
            _logger.LogInformation("Processing payment for order: {OrderId}", orderEvent.OrderId);
            var paymentResult = await _paymentService.ProcessPaymentAsync(
                orderEvent.OrderId, orderEvent.UserId, orderEvent.TotalPrice, cancellationToken);

            if (!paymentResult.IsSuccessful)
            {
                _logger.LogWarning("Payment failed for order: {OrderId}, Reason: {Reason}",
                    orderEvent.OrderId, paymentResult.FailureReason);
                await CompensateAndCancelOrderAsync(sagaState, paymentResult.FailureReason, cancellationToken);
                return;
            }

            sagaState.CurrentStep = OrderSagaStep.PaymentProcessed;
            sagaState.PaymentTransactionId = paymentResult.TransactionId;
            await stateRepository.UpdateSagaStateAsync(sagaState, cancellationToken);

            // Bước 4: Hoàn tất đơn hàng
            _logger.LogInformation("Completing order: {OrderId}", orderEvent.OrderId);
            await _orderService.MarkOrderAsPaidAsync(orderEvent.OrderId, cancellationToken);

            sagaState.CurrentStep = OrderSagaStep.OrderCompleted;
            sagaState.Status = SagaStatus.Completed;
            sagaState.CompletedAt = DateTime.UtcNow;
            await stateRepository.UpdateSagaStateAsync(sagaState, cancellationToken);

            // Gửi thông báo cho khách hàng
            await _notificationService.SendOrderConfirmationAsync(orderEvent.OrderId, orderEvent.UserId, cancellationToken);

            _logger.LogInformation("Order processing saga completed successfully for order: {OrderId}", orderEvent.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in order processing saga for order: {OrderId}", orderEvent.OrderId);
            await CompensateAndCancelOrderAsync(sagaState, $"Lỗi hệ thống: {ex.Message}", cancellationToken);
        }
    }

    public async Task ResumeFailedSagasAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var stateRepository = scope.ServiceProvider.GetRequiredService<ISagaStateRepository>();

        var pendingSagas = await stateRepository.GetPendingSagasAsync(cancellationToken);

        foreach (var saga in pendingSagas)
        {
            _logger.LogInformation("Resuming failed saga for order: {OrderId}, current step: {Step}",
                saga.OrderId, saga.CurrentStep);

        }
    }

    private async Task CompensateAndCancelOrderAsync(OrderSagaState sagaState, string reason, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var stateRepository = scope.ServiceProvider.GetRequiredService<ISagaStateRepository>();

        try
        {
            _logger.LogWarning("Starting compensation for failed saga, order: {OrderId}", sagaState.OrderId);

            // Hoàn tác tùy thuộc vào bước hiện tại
            switch (sagaState.CurrentStep)
            {
                case OrderSagaStep.PaymentProcessed:
                    await _paymentService.RefundPaymentAsync(sagaState.OrderId, sagaState.PaymentTransactionId, cancellationToken);
                    _logger.LogInformation("Payment refunded for order: {OrderId}", sagaState.OrderId);
                    goto case OrderSagaStep.InventoryReserved;

                case OrderSagaStep.InventoryReserved:
                    await _inventoryService.ReleaseInventoryAsync(sagaState.OrderId, cancellationToken);
                    _logger.LogInformation("Inventory released for order: {OrderId}", sagaState.OrderId);
                    goto case OrderSagaStep.OrderMarkedAsProcessing;

                case OrderSagaStep.OrderMarkedAsProcessing:
                    await _orderService.MarkOrderAsCancelledAsync(sagaState.OrderId, reason, cancellationToken);
                    _logger.LogInformation("Order marked as cancelled: {OrderId}", sagaState.OrderId);
                    break;
            }

            // Cập nhật trạng thái saga là thất bại
            sagaState.Status = SagaStatus.Failed;
            sagaState.FailureReason = reason;
            sagaState.CompletedAt = DateTime.UtcNow;
            await stateRepository.UpdateSagaStateAsync(sagaState, cancellationToken);

            // Thông báo cho khách hàng
            await _notificationService.SendOrderCancellationAsync(sagaState.OrderId, sagaState.UserId, reason, cancellationToken);

            _logger.LogInformation("Compensation completed for order: {OrderId}", sagaState.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during compensation for order: {OrderId}", sagaState.OrderId);
            // Lưu lỗi để xử lý thủ công sau
            sagaState.Status = SagaStatus.CompensationFailed;
            sagaState.FailureReason = $"Lỗi bồi hoàn: {ex.Message}";
            await stateRepository.UpdateSagaStateAsync(sagaState, cancellationToken);
        }
    }
}

