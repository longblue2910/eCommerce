// src/Services/InventoryService/Application/Services/IInventoryChangeSubscription.cs
using InventoryService.Application.DTOs;

namespace InventoryService.Application.Services;

/// <summary>
/// Interface cho đối tượng đăng ký theo dõi thay đổi tồn kho
/// </summary>
public interface IInventoryChangeSubscription
{
    Guid Id { get; }
    Task<bool> WaitForNextNotificationAsync(CancellationToken cancellationToken);
    InventoryChangeNotificationDto GetLatestNotification();
}
