// src/Services/InventoryService/Application/Services/Implementations/InventoryChangeSubscription.cs
using System.Threading.Channels;
using InventoryService.Application.DTOs;

namespace InventoryService.Application.Services.Implementations;

public class InventoryChangeSubscription : IInventoryChangeSubscription
{
    private readonly Channel<InventoryChangeNotificationDto> _channel;
    private readonly HashSet<Guid> _productIds;
    private InventoryChangeNotificationDto _latestNotification;

    public Guid Id { get; } = Guid.NewGuid();

    public InventoryChangeSubscription(List<Guid> productIds)
    {
        _productIds = new HashSet<Guid>(productIds);
        _channel = Channel.CreateUnbounded<InventoryChangeNotificationDto>(
            new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });
    }

    public async Task<bool> WaitForNextNotificationAsync(CancellationToken cancellationToken)
    {
        try
        {
            _latestNotification = await _channel.Reader.ReadAsync(cancellationToken);
            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    public InventoryChangeNotificationDto GetLatestNotification()
    {
        return _latestNotification;
    }

    public bool IsInterestedInProduct(Guid productId)
    {
        return _productIds.Contains(productId);
    }

    public async Task NotifyAsync(InventoryChangeNotificationDto notification, CancellationToken cancellationToken)
    {
        await _channel.Writer.WriteAsync(notification, cancellationToken);
    }
}
