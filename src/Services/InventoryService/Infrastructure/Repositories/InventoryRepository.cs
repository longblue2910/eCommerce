// src/Services/InventoryService/Infrastructure/Repositories/InventoryRepository.cs
using System.Text.Json;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Repositories;
using InventoryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryService.Infrastructure.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly InventoryDbContext _dbContext;
    private readonly ILogger<InventoryRepository> _logger;

    public InventoryRepository(InventoryDbContext dbContext, ILogger<InventoryRepository> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Inventory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Inventories
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        if (entity == null)
            return null;

        // Lấy reservations từ JSON
        LoadReservations(entity);
        return entity;
    }

    public async Task<Inventory?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Inventories
            .FirstOrDefaultAsync(i => i.ProductId == productId, cancellationToken);

        if (entity == null)
            return null;

        // Lấy reservations từ JSON
        LoadReservations(entity);
        return entity;
    }

    public async Task<List<Inventory>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        // Cần scan tất cả records vì Reservations là JSON
        var allInventories = await _dbContext.Inventories.ToListAsync(cancellationToken);
        var result = new List<Inventory>();

        foreach (var inventory in allInventories)
        {
            LoadReservations(inventory);
            if (inventory.Reservations.ContainsKey(orderId))
            {
                result.Add(inventory);
            }
        }

        return result;
    }

    public async Task<List<Inventory>> GetByProductIdsAsync(List<Guid> productIds, CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.Inventories
            .Where(i => productIds.Contains(i.ProductId))
            .ToListAsync(cancellationToken);

        foreach (var entity in entities)
        {
            LoadReservations(entity);
        }

        return entities;
    }

    public async Task CreateAsync(Inventory inventory, CancellationToken cancellationToken = default)
    {
        // Lưu Reservations dưới dạng JSON
        SaveReservations(inventory);

        await _dbContext.Inventories.AddAsync(inventory, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created inventory item for product {ProductId} with ID {Id}", inventory.ProductId, inventory.Id);
    }

    public async Task UpdateAsync(Inventory inventory, CancellationToken cancellationToken = default)
    {
        // Lưu Reservations dưới dạng JSON
        SaveReservations(inventory);

        _dbContext.Inventories.Update(inventory);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated inventory item for product {ProductId} with ID {Id}", inventory.ProductId, inventory.Id);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var inventory = await _dbContext.Inventories.FindAsync([id], cancellationToken);
        if (inventory == null)
            return false;

        _dbContext.Inventories.Remove(inventory);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted inventory item with ID {Id}", id);
        return true;
    }

    private void LoadReservations(Inventory inventory)
    {
        try
        {
            var reservationsJson = _dbContext.Entry(inventory).Property<string>("ReservationsJson").CurrentValue;
            if (!string.IsNullOrEmpty(reservationsJson))
            {
                var reservations = JsonSerializer.Deserialize<Dictionary<Guid, int>>(reservationsJson);
                if (reservations != null)
                {
                    // Giả sử có phương thức để thiết lập Reservations trực tiếp
                    // Hoặc tạo Reservation cho từng cặp key-value
                    foreach (var (orderId, quantity) in reservations)
                    {
                        inventory.Reserve(orderId, quantity);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deserializing reservations for inventory {InventoryId}", inventory.Id);
        }
    }

    private void SaveReservations(Inventory inventory)
    {
        try
        {
            var reservationsJson = JsonSerializer.Serialize(inventory.Reservations);
            _dbContext.Entry(inventory).Property<string>("ReservationsJson").CurrentValue = reservationsJson;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error serializing reservations for inventory {InventoryId}", inventory.Id);
        }
    }
}
