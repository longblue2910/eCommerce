namespace Domain.Events;

using SharedKernel.Common;
using System;

/// <summary>
/// Phát ra khi một yêu cầu hoàn tiền được tạo.
/// </summary>
public record RefundRequestedEvent(Guid RefundId, Guid OrderId, decimal Amount) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Phát ra khi yêu cầu hoàn tiền được chấp nhận.
/// </summary>
public record RefundApprovedEvent(Guid RefundId, Guid OrderId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Phát ra khi yêu cầu hoàn tiền bị từ chối.
/// </summary>
public record RefundRejectedEvent(Guid RefundId, Guid OrderId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
