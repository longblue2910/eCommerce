using Microsoft.Extensions.Options;
using OrderSaga.Worker.Entities;
using OrderSaga.Worker.Orchestrator;
using OrderSaga.Worker.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace OrderSaga.Worker.Consumers;

/// <summary>
/// Consumer lắng nghe sự kiện đơn hàng được tạo từ RabbitMQ và khởi động Saga xử lý đơn hàng
/// </summary>
public class OrderCreatedConsumer : BackgroundService
{
    private readonly ILogger<OrderCreatedConsumer> _logger;
    private readonly RabbitMQSettings _settings;
    private readonly ISagaOrchestrator _orchestrator;

    /// <summary>
    /// Constructor khởi tạo OrderCreatedConsumer
    /// </summary>
    /// <param name="logger">Logger để ghi log</param>
    /// <param name="settings">Cấu hình kết nối RabbitMQ</param>
    /// <param name="orchestrator">Orchestrator để điều phối quy trình Saga</param>
    public OrderCreatedConsumer(
        ILogger<OrderCreatedConsumer> logger,
        IOptions<RabbitMQSettings> settings,
        ISagaOrchestrator orchestrator)
    {
        _logger = logger;
        _settings = settings.Value;
        _orchestrator = orchestrator;
    }

    /// <summary>
    /// Phương thức xử lý chính của BackgroundService, chạy khi service được khởi động
    /// </summary>
    /// <param name="stoppingToken">Token để hủy thực thi khi service dừng</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.Host,
            UserName = _settings.Username,
            Password = _settings.Password,
            Port = _settings.Port
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        // Khai báo exchange loại Direct để định tuyến message dựa trên routing key
        channel.ExchangeDeclare(
            exchange: "order_saga_exchange",
            type: ExchangeType.Direct, // Change from ExchangeType.Topic to ExchangeType.Direct
            durable: true); // durable=true để đảm bảo exchange tồn tại sau khi RabbitMQ restart

        channel.QueueDeclare(
            queue: "order_saga_queue",
            durable: true,
            exclusive: false,
            autoDelete: false);

        channel.QueueBind(
            queue: "order_saga_queue",
            exchange: "order_saga_exchange",
            routingKey: "order.created");

        _logger.LogInformation("Đang lắng nghe sự kiện order.created");

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += async (sender, eventArgs) =>
        {
            try
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Nhận được sự kiện order.created: {Message}", message);

                var orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedIntegrationEvent>(message);

                if (orderCreatedEvent != null)
                {
                    await _orchestrator.StartOrderProcessingSaga(orderCreatedEvent, stoppingToken);
                    channel.BasicAck(eventArgs.DeliveryTag, false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý message");
                channel.BasicNack(eventArgs.DeliveryTag, false, true);
            }
        };

        channel.BasicConsume(
            queue: "order_saga_queue",
            autoAck: false,
            consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}
/** Sơ đồ luồng xử lý:
 Người dùng A đặt hàng ──→ Order API ──→ RabbitMQ Queue
                                              │
Người dùng B đặt hàng ──→ Order API ──→ RabbitMQ Queue
                                              │
                                              ↓
                                      OrderSaga Consumer
                                              │
                                              ↓
                                     Xử lý message của A
                                              │
                                              ↓
                                    Kiểm tra tồn kho cho A
                                              │
                                              ↓
                                     Giữ hàng thành công
                                              │
                                              ↓
                                     Xử lý message của B
                                              │
                                              ↓
                                    Kiểm tra tồn kho cho B
                                              │
                                              ↓
                                     Phản hồi hết hàng

 
 */