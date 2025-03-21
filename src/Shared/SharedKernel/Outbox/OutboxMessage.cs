namespace SharedKernel.Outbox;

using System;
using Newtonsoft.Json;

public class OutboxMessage
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string EventType { get; private set; }
    public string Payload { get; private set; }
    public bool Processed { get; private set; } = false;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private OutboxMessage() { }

    public OutboxMessage(string eventType, object @event)
    {
        EventType = eventType;
        Payload = JsonConvert.SerializeObject(@event);
    }

    public void MarkAsProcessed()
    {
        Processed = true;
    }
}
