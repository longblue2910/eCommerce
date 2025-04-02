using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace SharedKernel.Messaging;

/// <summary>
/// RabbitMQ Publisher để gửi message vào OrderSaga
/// </summary>
public class RabbitMqPublisher(IConfiguration configuration)
{
    private readonly string _hostName = configuration["RabbitMq:Host"];
    private readonly string _userName = configuration["RabbitMq:Username"];
    private readonly string _password = configuration["RabbitMq:Password"];
    private readonly string _exchangeName = "order_saga_exchange";
    private readonly int _port = int.Parse(configuration["RabbitMq:Port"]);

    /// <summary>
    /// Gửi message lên RabbitMQ để OrderSaga xử lý
    /// </summary>
    /// <typeparam name="T">Kiểu sự kiện</typeparam>
    /// <param name="event">Sự kiện cần gửi</param>
    /// <param name="routingKey">Routing key để OrderSaga xử lý</param>
    /// <param name="cancellationToken">Token hủy</param>
    public async Task PublishAsync<T>(T @event, string routingKey, CancellationToken cancellationToken = default) where T : class
    {
        var factory = new ConnectionFactory()
        {
            HostName = _hostName,
            Port = _port,
            UserName = _userName,
            Password = _password
        };

        await using var connection = await factory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync();

        // 🔹 Khai báo exchange loại "direct"
        await channel.ExchangeDeclareAsync(
            exchange: _exchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message).AsMemory();

        var properties = new BasicProperties
        {
            DeliveryMode = DeliveryModes.Persistent
        };

        // 🎯 Sử dụng routingKey để gửi đúng loại sự kiện vào OrderSaga
        await channel.BasicPublishAsync(
            exchange: _exchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken
        );
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