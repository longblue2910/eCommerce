namespace Domain.Events;

using SharedKernel.Common;
using System;

public record OrderCreatedEvent(Guid OrderId, Guid UserId, decimal TotalPrice) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record OrderPaymentStartedEvent(Guid OrderId, decimal Amount) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record OrderPaymentCompletedEvent(Guid OrderId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record OrderShippedEvent(Guid OrderId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record OrderCancelledEvent(Guid OrderId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record OrderRefundRequestedEvent(Guid OrderId, decimal Amount) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record OrderRefundCompletedEvent(Guid OrderId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
