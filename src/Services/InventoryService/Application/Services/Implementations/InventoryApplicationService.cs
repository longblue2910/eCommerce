// src/Services/InventoryService/Application/Services/Implementations/InventoryApplicationService.cs
using InventoryService.Application.DTOs;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace InventoryService.Application.Services.Implementations;

public class InventoryApplicationService : IInventoryApplicationService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IInventoryHistoryRepository _historyRepository;
    private readonly IDistributedCache _cache;
    private readonly ILogger<InventoryApplicationService> _logger;
    private readonly Dictionary<Guid, InventoryChangeSubscription> _subscriptions = new();

    public InventoryApplicationService(
        IInventoryRepository inventoryRepository,
        IInventoryHistoryRepository historyRepository,
        IDistributedCache cache,
        ILogger<InventoryApplicationService> logger)
    {
        _inventoryRepository = inventoryRepository;
        _historyRepository = historyRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<ProductInventoryStatusDto>> CheckInventoryAsync(
        List<ProductQuantityDto> products,
        CancellationToken cancellationToken = default)
    {
        var result = new List<ProductInventoryStatusDto>();

        foreach (var productDto in products)
        {
            // Kiểm tra trong cache trước
            var cacheKey = $"inventory:{productDto.ProductId}";
            var inventoryJson = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(inventoryJson))
            {
                try
                {
                    var cachedInventory = JsonSerializer.Deserialize<InventoryDto>(inventoryJson);
                    result.Add(new ProductInventoryStatusDto
                    {
                        ProductId = productDto.ProductId,
                        InStock = cachedInventory.AvailableQuantity >= productDto.Quantity,
                        AvailableQuantity = cachedInventory.AvailableQuantity,
                        Sku = cachedInventory.Sku,
                        Name = cachedInventory.ProductName
                    });
                    continue;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi xử lý dữ liệu cache cho sản phẩm {ProductId}", productDto.ProductId);
                }
            }

            // Nếu không có trong cache, query từ database
            var inventory = await _inventoryRepository.GetByProductIdAsync(productDto.ProductId, cancellationToken);

            if (inventory != null)
            {
                // Lấy thông tin tên sản phẩm (giả định)
                var productName = "Product " + productDto.ProductId.ToString().Substring(0, 5);

                result.Add(new ProductInventoryStatusDto
                {
                    ProductId = productDto.ProductId,
                    InStock = inventory.AvailableQuantity >= productDto.Quantity,
                    AvailableQuantity = inventory.AvailableQuantity,
                    Sku = inventory.Sku,
                    Name = productName
                });

                // Lưu vào cache
                var inventoryDto = new InventoryDto
                {
                    ProductId = inventory.ProductId,
                    TotalQuantity = inventory.TotalQuantity,
                    ReservedQuantity = inventory.ReservedQuantity,
                    AvailableQuantity = inventory.AvailableQuantity,
                    Sku = inventory.Sku,
                    ProductName = productName
                };

                await _cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(inventoryDto),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    },
                    cancellationToken);
            }
            else
            {
                result.Add(new ProductInventoryStatusDto
                {
                    ProductId = productDto.ProductId,
                    InStock = false,
                    AvailableQuantity = 0,
                    Sku = string.Empty,
                    Name = "Unknown Product"
                });
            }
        }

        return result;
    }

    public async Task<List<ProductReservationResultDto>> ReserveInventoryAsync(
        Guid orderId,
        List<ProductQuantityDto> products,
        CancellationToken cancellationToken = default)
    {
        var result = new List<ProductReservationResultDto>();

        try
        {
            foreach (var productDto in products)
            {
                var inventory = await _inventoryRepository.GetByProductIdAsync(productDto.ProductId, cancellationToken);

                if (inventory == null)
                {
                    result.Add(new ProductReservationResultDto
                    {
                        ProductId = productDto.ProductId,
                        Reserved = false,
                        Message = "Product not found"
                    });
                    continue;
                }

                var success = inventory.Reserve(orderId, productDto.Quantity);

                if (success)
                {
                    await _inventoryRepository.UpdateAsync(inventory, cancellationToken);

                    // Ghi lịch sử thay đổi tồn kho
                    await _historyRepository.CreateAsync(new InventoryHistory(
                        inventoryId: inventory.Id,
                        orderId: orderId,
                        eventType: "RESERVED",
                        quantityChanged: productDto.Quantity,
                        newTotalQuantity: inventory.TotalQuantity,
                        newReservedQuantity: inventory.ReservedQuantity
                    ), cancellationToken);

                    // Cập nhật thông báo đến các subscriber
                    await NotifyInventoryChangedAsync(
                        productDto.ProductId,
                        inventory.AvailableQuantity,
                        "RESERVED",
                        cancellationToken);

                    // Xóa cache
                    await _cache.RemoveAsync($"inventory:{productDto.ProductId}", cancellationToken);

                    result.Add(new ProductReservationResultDto
                    {
                        ProductId = productDto.ProductId,
                        Reserved = true,
                        Message = "Successfully reserved"
                    });
                }
                else
                {
                    result.Add(new ProductReservationResultDto
                    {
                        ProductId = productDto.ProductId,
                        Reserved = false,
                        Message = "Insufficient stock"
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi giữ hàng cho đơn hàng {OrderId}", orderId);
            throw;
        }

        return result;
    }

    public async Task<bool> ReleaseInventoryAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var inventories = await _inventoryRepository.GetByOrderIdAsync(orderId, cancellationToken);

            foreach (var inventory in inventories)
            {
                if (inventory.Release(orderId))
                {
                    await _inventoryRepository.UpdateAsync(inventory, cancellationToken);

                    // Ghi lịch sử thay đổi tồn kho
                    await _historyRepository.CreateAsync(new InventoryHistory(
                        inventoryId: inventory.Id,
                        orderId: orderId,
                        eventType: "RELEASED",
                        quantityChanged: -inventory.Reservations[orderId], // Giả định dictionary có key này
                        newTotalQuantity: inventory.TotalQuantity,
                        newReservedQuantity: inventory.ReservedQuantity
                    ), cancellationToken);

                    // Cập nhật thông báo đến các subscriber
                    await NotifyInventoryChangedAsync(
                        inventory.ProductId,
                        inventory.AvailableQuantity,
                        "RELEASED",
                        cancellationToken);

                    // Xóa cache
                    await _cache.RemoveAsync($"inventory:{inventory.ProductId}", cancellationToken);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi giải phóng hàng cho đơn hàng {OrderId}", orderId);
            return false;
        }
    }

    public async Task<bool> ConfirmInventoryAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var inventories = await _inventoryRepository.GetByOrderIdAsync(orderId, cancellationToken);

            foreach (var inventory in inventories)
            {
                if (inventory.Confirm(orderId))
                {
                    await _inventoryRepository.UpdateAsync(inventory, cancellationToken);

                    // Ghi lịch sử thay đổi tồn kho
                    await _historyRepository.CreateAsync(new InventoryHistory(
                        inventoryId: inventory.Id,
                        orderId: orderId,
                        eventType: "CONFIRMED",
                        quantityChanged: -inventory.Reservations[orderId], // Giả định dictionary có key này
                        newTotalQuantity: inventory.TotalQuantity,
                        newReservedQuantity: inventory.ReservedQuantity
                    ), cancellationToken);

                    // Cập nhật thông báo đến các subscriber
                    await NotifyInventoryChangedAsync(
                        inventory.ProductId,
                        inventory.AvailableQuantity,
                        "CONFIRMED",
                        cancellationToken);

                    // Xóa cache
                    await _cache.RemoveAsync($"inventory:{inventory.ProductId}", cancellationToken);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xác nhận sử dụng hàng cho đơn hàng {OrderId}", orderId);
            return false;
        }
    }

    public async Task<IInventoryChangeSubscription> SubscribeToInventoryChangesAsync(
        List<Guid> productIds,
        CancellationToken cancellationToken = default)
    {
        var subscription = new InventoryChangeSubscription(productIds);
        _subscriptions.Add(subscription.Id, subscription);
        return subscription;
    }

    public Task UnsubscribeFromInventoryChangesAsync(
        Guid subscriptionId,
        CancellationToken cancellationToken = default)
    {
        if (_subscriptions.ContainsKey(subscriptionId))
        {
            _subscriptions.Remove(subscriptionId);
        }

        return Task.CompletedTask;
    }

    public async Task<List<InventoryHistoryDto>> GetInventoryHistoryAsync(
        Guid productId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var inventory = await _inventoryRepository.GetByProductIdAsync(productId, cancellationToken);

        if (inventory == null)
        {
            return new List<InventoryHistoryDto>();
        }

        var history = await _historyRepository.GetByInventoryIdAsync(
            inventory.Id,
            startDate,
            endDate,
            cancellationToken);

        return history.Select(h => new InventoryHistoryDto
        {
            ProductId = productId,
            NewQuantity = h.NewTotalQuantity - h.NewReservedQuantity,
            EventType = h.EventType,
            Timestamp = h.Timestamp
        }).ToList();
    }

    private async Task NotifyInventoryChangedAsync(
        Guid productId,
        int newQuantity,
        string eventType,
        CancellationToken cancellationToken)
    {
        foreach (var subscription in _subscriptions.Values)
        {
            if (subscription.IsInterestedInProduct(productId))
            {
                await subscription.NotifyAsync(new InventoryChangeNotificationDto
                {
                    ProductId = productId,
                    NewQuantity = newQuantity,
                    EventType = eventType,
                    Timestamp = DateTime.UtcNow
                }, cancellationToken);
            }
        }
    }
}
