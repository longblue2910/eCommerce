using MassTransit;
using SharedKernel.Messaging;

public class MassTransitMessageBus(IPublishEndpoint publishEndpoint) : IMessageBus
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task Publish<T>(T message) where T : class
    {
        await _publishEndpoint.Publish(message);
    }
}
