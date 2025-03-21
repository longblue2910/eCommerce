using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderSaga.Messaging;

public class RabbitMqMessageBus : IMessageBus
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqMessageBus(IConfiguration configuration)
    {
        var rabbitMqConfig = configuration.GetSection("RabbitMQ");

        var factory = new ConnectionFactory
        {
            HostName = rabbitMqConfig["HostName"],
            Port = int.Parse(rabbitMqConfig["Port"] ?? "5672"),
            UserName = rabbitMqConfig["UserName"],
            Password = rabbitMqConfig["Password"],
            VirtualHost = rabbitMqConfig["VirtualHost"]
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public Task PublishAsync<T>(string queueName, T message)
    {
        return Task.Run(() =>
        {
            _channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _channel.BasicPublish("", queueName, null, body);
        });
    }

    public Task SubscribeAsync<T>(string queueName, Func<T, Task> handler)
    {
        return Task.Run(() =>
        {
            _channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(body));
                await handler(message);
            };
            _channel.BasicConsume(queueName, true, consumer);
        });
    }
}
