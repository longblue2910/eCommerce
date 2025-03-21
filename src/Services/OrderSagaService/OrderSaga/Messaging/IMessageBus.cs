namespace OrderSaga.Messaging;

public interface IMessageBus
{
    Task PublishAsync<T>(string queueName, T message);
    Task SubscribeAsync<T>(string queueName, Func<T, Task> handler);
}
