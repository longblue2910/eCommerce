using MassTransit;
using OrderSaga.Worker.Orchestrator;
using SharedKernel.Events;

namespace OrderSaga.Worker.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedIntegrationEvent>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;
    private readonly ISagaOrchestrator _orchestrator;

    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger, ISagaOrchestrator orchestrator)
    {
        _logger = logger;
        _orchestrator = orchestrator;
    }

    public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
    {
        try
        {
            var message = context.Message;
            _logger.LogInformation("Received OrderCreatedIntegrationEvent: OrderId={OrderId}, UserId={UserId}",
                message.OrderId, message.UserId);

            await _orchestrator.StartOrderProcessingSaga(message, context.CancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing OrderCreatedIntegrationEvent");
            throw;
        }
    }
}
