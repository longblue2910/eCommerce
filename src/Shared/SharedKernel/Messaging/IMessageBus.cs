namespace SharedKernel.Messaging;

public interface IMessageBus
{
    Task Publish<T>(T message) where T : class;
}
