// src/Services/InventoryService/Domain/Repositories/IInventoryHistoryRepository.cs
namespace InventoryService.Domain.Repositories;

using InventoryService.Domain.Entities;

public interface IInventoryHistoryRepository
{
    Task<List<InventoryHistory>> GetByInventoryIdAsync(Guid inventoryId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task CreateAsync(InventoryHistory history, CancellationToken cancellationToken = default);
}
