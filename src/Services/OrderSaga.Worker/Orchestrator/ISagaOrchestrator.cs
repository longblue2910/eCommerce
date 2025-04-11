using OrderSaga.Worker.Entities;
using SharedKernel.Events;

namespace OrderSaga.Worker.Orchestrator;

/// <summary>
/// Orchestrator điều phối luồng Saga xử lý đơn hàng
/// </summary>
public interface ISagaOrchestrator
{
    /// <summary>
    /// Bắt đầu quy trình xử lý đơn hàng
    /// </summary>
    Task StartOrderProcessingSaga(OrderCreatedIntegrationEvent orderEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Xử lý tiếp các Saga đã bị lỗi hoặc tạm dừng
    /// </summary>
    Task ResumeFailedSagasAsync(CancellationToken cancellationToken = default);
}
