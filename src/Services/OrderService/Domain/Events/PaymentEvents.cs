namespace Domain.Events;

using SharedKernel.Common;
using System;

/// <summary>
/// Phát ra khi một thanh toán mới được tạo.
/// </summary>
public record PaymentCreatedEvent(Guid PaymentId, Guid OrderId, decimal Amount) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Phát ra khi thanh toán thành công.
/// </summary>
public record PaymentCompletedEvent(Guid PaymentId, Guid OrderId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Phát ra khi thanh toán thất bại.
/// </summary>
public record PaymentFailedEvent(Guid PaymentId, Guid OrderId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
