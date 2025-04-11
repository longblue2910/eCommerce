using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace SharedKernel.Messaging;

/// <summary>
/// RabbitMQ Publisher để gửi message vào OrderSaga
/// </summary>
public class RabbitMqPublisher
{
    private readonly string _hostName;
    private readonly string _userName;
    private readonly string _password;
    private readonly string _exchangeName = "order_saga_exchange";
    private readonly int _port;

    public RabbitMqPublisher(IConfiguration configuration)
    {
        _hostName = configuration["RabbitMq:Host"];
        _userName = configuration["RabbitMq:Username"];
        _password = configuration["RabbitMq:Password"];
        _port = int.Parse(configuration["RabbitMq:Port"]);
    }

    /// <summary>
    /// Gửi message lên RabbitMQ để OrderSaga xử lý
    /// </summary>
    /// <typeparam name="T">Kiểu sự kiện</typeparam>
    /// <param name="event">Sự kiện cần gửi</param>
    /// <param name="routingKey">Routing key để OrderSaga xử lý</param>
    public void Publish<T>(T @event, string routingKey) where T : class
    {
        var factory = new ConnectionFactory
        {
            HostName = _hostName,
            Port = _port,
            UserName = _userName,
            Password = _password
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(
            exchange: _exchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false);

        // Tạo message envelope tương thích với MassTransit
        var messageType = typeof(T).FullName;
        var envelope = new
        {
            messageId = Guid.NewGuid(),
            messageType = new[] { messageType },
            message = @event,
            sentTime = DateTime.UtcNow
        };

        var message = JsonSerializer.Serialize(envelope, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        var body = Encoding.UTF8.GetBytes(message);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";

        // Thêm các header cần thiết cho MassTransit
        properties.Headers = new Dictionary<string, object>
    {
        { "Content-Type", "application/json" },
        { "MessageType", messageType }
    };

        channel.BasicPublish(
            exchange: _exchangeName,
            routingKey: routingKey,
            basicProperties: properties,
            body: body);

        Console.WriteLine($"Message published to exchange '{_exchangeName}' with routing key '{routingKey}'");
    }

}

/* Mô hình trực quan Mô hình Direct Exchange
- Direct Exchange là một loại exchange trong RabbitMQ
    + Nơi các message được định tuyến đến các queue dựa trên một routing key khớp chính xác. 
    + Điều này có nghĩa là message sẽ chỉ được gửi đến queue có routing key trùng khớp với routing key của message.
- Producer (Publisher) -> [Direct Exchange] -> [Queue]

+----------------+       +------------------+       +----------------+       +----------------+
|    Producer    | ----> |  Direct Exchange | ----> |     Queue      | ----> |    Consumer    |
|  (Publisher)   |       |  (order_exchange)|       | (order_queue)  |       |                |
+----------------+       +------------------+       +----------------+       +----------------+
       |                         |                          |                          |
       |                         |                          |                          |
       |                         |                          |                          |
       |                         |                          |                          |
       +------------------------>+                          |                          |
       |  Routing Key: "order.created"                      |                          |
       |                                                    |                          |
       |                                                    |                          |
       +--------------------------------------------------->+                          |
       |  Message: { OrderId: 123, UserId: 456, TotalPrice: 789.99 }                   |
       |                                                                               |
       |                                                                               |
       +------------------------------------------------------------------------------>+
                                                                                       |
                                                                                       |
                                                                                       v
                                                                               +----------------+
                                                                               |    Consumer    |
                                                                               |                |
                                                                               +----------------+


Giải thích chi tiết:
1.	Producer (Publisher): Gửi message với routing key order.created đến order_exchange.
2.	Direct Exchange: Nhận message và kiểm tra routing key. 
    Nếu routing key khớp với order.created, message sẽ được gửi đến order_queue.
3.	Queue: order_queue lưu trữ message cho đến khi consumer nhận và xử lý.
4.	Consumer: Lắng nghe order_queue và xử lý message khi nhận được.

 */